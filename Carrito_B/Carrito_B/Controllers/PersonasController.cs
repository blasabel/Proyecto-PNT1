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
    [Authorize(Roles = Configs.ADMIN_ROLE)]
    public class PersonasController : Controller
    {
        private readonly CarritoContext _context;
        private readonly UserManager<Persona> _userManager;

        public PersonasController(CarritoContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Personas
        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var persona = await _userManager.FindByIdAsync(id.ToString());
            if (persona == null) return NotFound();

            return View(persona);
        }

        // GET: Personas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Nombre,Apellido,DNI,Telefono,Direccion,Email")] Persona persona)
        {
            if (!ModelState.IsValid) return View(persona);

            persona.UserName = persona.Email;
            persona.NormalizedEmail = persona.Email?.ToUpperInvariant();
            persona.NormalizedUserName = persona.NormalizedEmail;
            persona.FechaAlta = DateTime.Now;

            var password = Request.Form["Password"].FirstOrDefault();

            IdentityResult createResult;
            if (!string.IsNullOrEmpty(password))
            {
                createResult = await _userManager.CreateAsync(persona, password);
            }
            else
            {
                createResult = await _userManager.CreateAsync(persona);
            }

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(persona);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var persona = await _userManager.FindByIdAsync(id.ToString());
            if (persona == null) return NotFound();

            return View(persona);
        }

        // POST: Personas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Nombre,Apellido,DNI,Telefono,Direccion,Email")] Persona persona)
        {
            if (id != persona.Id) return NotFound();

            if (!ModelState.IsValid) return View(persona);

            var personaEnBD = await _userManager.FindByIdAsync(persona.Id.ToString());
            if (personaEnBD == null) return NotFound();

            personaEnBD.UserName = persona.UserName;
            personaEnBD.Nombre = persona.Nombre;
            personaEnBD.Apellido = persona.Apellido;
            personaEnBD.DNI = persona.DNI;
            personaEnBD.Telefono = persona.Telefono;
            personaEnBD.Direccion = persona.Direccion;

            var nuevoNormalized = persona.Email?.ToUpperInvariant();
            if (!string.Equals(personaEnBD.NormalizedEmail, nuevoNormalized, StringComparison.Ordinal))
            {
                var exists = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == nuevoNormalized && u.Id != persona.Id);
                if (exists)
                {
                    ModelState.AddModelError("Email", "El email ya está en uso");
                    return View(persona);
                }

                personaEnBD.Email = persona.Email;
                personaEnBD.NormalizedEmail = nuevoNormalized;
                personaEnBD.UserName = persona.Email;
                personaEnBD.NormalizedUserName = nuevoNormalized;
            }

            var updateResult = await _userManager.UpdateAsync(personaEnBD);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(persona);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Personas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var persona = await _userManager.FindByIdAsync(id.ToString());
            if (persona == null) return NotFound();

            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _userManager.FindByIdAsync(id.ToString());
            if (persona != null)
            {
                var deleteResult = await _userManager.DeleteAsync(persona);
                if (!deleteResult.Succeeded)
                {
                    foreach (var error in deleteResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(persona);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PersonaExistsAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user != null;
        }
    }
}
