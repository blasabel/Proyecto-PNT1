using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carrito_B.Data;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authorization;
using Carrito_B.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Carrito_B.Controllers
{
    public class ClientesController : Controller
    {
        private readonly CarritoContext _context;
        private readonly UserManager<Persona> _userManager;

        public ClientesController(CarritoContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Clientes
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.OfType<Cliente>().ToListAsync());
        }

        // GET: Clientes/Details/5
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }    
               
            var cliente = await _userManager.FindByIdAsync(id.ToString()) as Cliente;
            if (cliente == null)
            {
                return NotFound();
            }    
            return View(cliente);
        }

        // GET: Clientes/Create
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        public async Task<IActionResult> Create([Bind("IdentificacionUnica,Id,UserName,Nombre,Apellido,DNI,Telefono,Direccion,Email,Password")] Cliente cliente)
        {
            if (!ModelState.IsValid) return View(cliente);

            cliente.UserName = cliente.Email;
            cliente.NormalizedEmail = cliente.Email?.ToUpperInvariant();
            cliente.NormalizedUserName = cliente.NormalizedEmail;

            if (await EmailExistsAsync(cliente.Email))
            {
                ModelState.AddModelError("Email", "El email ya está en uso");
                return View(cliente);
            }

            cliente.FechaAlta = DateTime.Now;

           
            var password = Request.Form["Password"].FirstOrDefault();

            IdentityResult createResult;
            if (!string.IsNullOrEmpty(password))
            {
                createResult = await _userManager.CreateAsync(cliente, password);
            }
            else
            {
                createResult = await _userManager.CreateAsync(cliente); 
            }

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(cliente);
            }
            await _userManager.AddToRoleAsync(cliente, Configs.CLIENTE_ROLE);

           
            var tieneActivo = _context.Carritos.Any(c => c.ClienteId == cliente.Id && c.Activo);
            if (!tieneActivo)
            {
                var nuevoCarrito = new Carrito
                {
                    Activo = true,
                    ClienteId = cliente.Id,
                    CarritoItems = new System.Collections.Generic.List<CarritoItem>()
                };
                _context.Carritos.Add(nuevoCarrito);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

		// GET: Clientes/Edit/5
		[Authorize(Roles = Configs.ADMIN_ROLE)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }    
                
            var cliente = await _userManager.FindByIdAsync(id.ToString()) as Cliente;
            if (cliente == null)
            {
                return NotFound();
            }      
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Configs.ADMIN_ROLE)]
		public async Task<IActionResult> Edit(int id, [Bind("IdentificacionUnica,Id,UserName,Nombre,Apellido,DNI,Telefono,Direccion,Email")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }   
               

            var clienteEnDB = await _userManager.FindByIdAsync(cliente.Id.ToString()) as Cliente;
            if (clienteEnDB == null)
            {
                return NotFound();
            }
               

            if (id != int.Parse(_userManager.GetUserId(User))) 
            { 
                return NotFound(); 
            }

            if (User.IsInRole(Configs.CLIENTE_ROLE)) 
            {
                clienteEnDB.UserName = cliente.UserName;
                clienteEnDB.Telefono = cliente.Telefono; 
                clienteEnDB.Direccion = cliente.Direccion; 
            } else if (User.IsInRole(Configs.ADMIN_ROLE)) 
            { 
              clienteEnDB.Nombre = cliente.Nombre; 
              clienteEnDB.Apellido = cliente.Apellido; 
              clienteEnDB.DNI = cliente.DNI; 
              clienteEnDB.IdentificacionUnica = cliente.IdentificacionUnica; 
              clienteEnDB.Email = cliente.Email;
            }


            var updateResult = await _userManager.UpdateAsync(clienteEnDB);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(cliente);
            }

            return RedirectToAction(nameof(Index));
        }

		// GET: Clientes/Delete/5
		[Authorize(Roles = Configs.ADMIN_ROLE)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _userManager.FindByIdAsync(id.ToString()) as Cliente;
            if (cliente == null)
            {
                return NotFound();
            }    
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Configs.ADMIN_ROLE)]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _userManager.FindByIdAsync(id.ToString()) as Cliente;
            if (cliente != null)
            {
                var deleteResult = await _userManager.DeleteAsync(cliente);
                if (!deleteResult.Succeeded)
                {
                    foreach (var error in deleteResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(cliente);
                }
            }

            return RedirectToAction(nameof(Index));
        }
       
        private async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var normalized = email.ToUpperInvariant();
            return await _userManager.Users
                .OfType<Cliente>()
                .AnyAsync(c => c.NormalizedEmail == normalized && c.Id != (excludeId ?? 0));
        }

        private async Task<bool> DNIExistsAsync(string dni, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(dni)) return false;

            var normalized = dni.ToUpperInvariant();
            return await _userManager.Users
                .OfType<Cliente>()
                .AnyAsync(c => c.NormalizedEmail == normalized && c.Id != (excludeId ?? 0));
        }

        // GET: Clientes/Historial/5
        [Authorize(Roles = Configs.CLIENTE_ROLE)]
        public async Task<IActionResult> Historial()
        {
			var userId = _userManager.GetUserId(User);
			if (userId == null) return NotFound();

			var clienteId = int.Parse(userId);

			var compras = await _context.Compras
                .Include(c => c.Sucursal)
                .Include(c => c.Carrito)
                .Where(c => c.Carrito.ClienteId == clienteId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return View(compras);
        }
    }
}
