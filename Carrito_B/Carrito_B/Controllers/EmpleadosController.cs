using Carrito_B.Data;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Carrito_B.Controllers
{
    [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

    public class EmpleadosController : Controller
    {
        private readonly CarritoContext _context;
        private readonly UserManager<Persona> _userManager;

        public EmpleadosController(CarritoContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.OfType<Empleado>().ToListAsync());
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _userManager.FindByIdAsync(id.ToString()) as Empleado;
            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Nombre,Apellido,DNI,Telefono,Direccion,Email")] Empleado empleado)
        {
            if (!ModelState.IsValid) return View(empleado);

            empleado.FechaAlta = DateTime.Now;

            
            if (_userManager.Users.Any(u => u.DNI == empleado.DNI))
            {
                ModelState.AddModelError("DNI", "Ya existe un usuario con este DNI.");
                return View(empleado);
            }

            if (_userManager.Users.Any(u => u.Email == empleado.Email))
            {
                ModelState.AddModelError("Email", "Este email ya está registrado.");
                return View(empleado);
            }


            var ultimoLegajo = await _userManager.Users
                .OfType<Empleado>()
                .OrderByDescending(e => e.Legajo)
                .Select(e => e.Legajo)
                .FirstOrDefaultAsync();

            empleado.Legajo = ultimoLegajo > 0 ? ultimoLegajo + 1 : 10;

            var createResult = await _userManager.CreateAsync(empleado, Configs.EMPLEADO_PASSWORD);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(empleado);
            }

            await _userManager.AddToRoleAsync(empleado, Configs.EMPLEADO_ROLE);

            return RedirectToAction(nameof(Index));
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _userManager.FindByIdAsync(id.ToString()) as Empleado;
            if (empleado == null) return NotFound();
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Nombre,Apellido,DNI,Telefono,Direccion,Email,Legajo")] Empleado empleado)
        {
            if (id != empleado.Id) return NotFound();

            if (!ModelState.IsValid) return View(empleado);

            var empleadoEnDB = await _userManager.FindByIdAsync(empleado.Id.ToString()) as Empleado;
            if (empleadoEnDB == null) return NotFound();

            empleadoEnDB.UserName = empleado.UserName;
            empleadoEnDB.Nombre = empleado.Nombre;
            empleadoEnDB.Apellido = empleado.Apellido;
            empleadoEnDB.DNI = empleado.DNI;
            empleadoEnDB.Telefono = empleado.Telefono;
            empleadoEnDB.Direccion = empleado.Direccion;
            empleadoEnDB.Email = empleado.Email;
            empleadoEnDB.Legajo = empleado.Legajo;

            var updateResult = await _userManager.UpdateAsync(empleadoEnDB);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(empleado);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _userManager.FindByIdAsync(id.ToString()) as Empleado;
            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _userManager.FindByIdAsync(id.ToString()) as Empleado;
            if (empleado != null)
            {
                var deleteResult = await _userManager.DeleteAsync(empleado);
                if (!deleteResult.Succeeded)
                {
                    foreach (var error in deleteResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(empleado);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmpleadoExistsAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user != null;
        }
    }
}
