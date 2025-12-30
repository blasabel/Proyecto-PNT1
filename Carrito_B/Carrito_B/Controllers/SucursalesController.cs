using Carrito_B.Data;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Carrito_B.Controllers
{
    public class SucursalesController : Controller
    {
        private readonly CarritoContext _context;

        public SucursalesController(CarritoContext context)
        {
            _context = context;
        }

        // GET: Sucursales
        public IActionResult Index()
        {
            if (!_context.Sucursales.Any())
            {
                var seed = GetSucursalsSeed();

                _context.Sucursales.AddRange(seed);
                _context.SaveChanges();
            }

            var sucursales = _context.Sucursales.ToList();
            return View(sucursales);
        }

        // GET: Sucursales/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var sucursal = _context.Sucursales
                                   .FirstOrDefault(m => m.Id == id);
            if (sucursal == null) return NotFound();

            return View(sucursal);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // GET: Sucursales/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // POST: Sucursales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nombre,Direccion,Email,Activa,Telefono")] Sucursal sucursal)
        {
            if (_context.Sucursales.Any(s => s.Nombre.ToLower() == sucursal.Nombre.ToLower()))
            {
                ModelState.AddModelError("Nombre", "Ya existe una sucursal con ese nombre.");
                return View(sucursal);
            }

            if (ModelState.IsValid)
            {
                _context.Add(sucursal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(sucursal);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // GET: Sucursales/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var sucursal = _context.Sucursales.Find(id);
            if (sucursal == null) return NotFound();

            return View(sucursal);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nombre,Direccion,Email,Activa,Telefono")] Sucursal sucursal)
        {
            if (id != sucursal.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (!sucursal.Activa)
                {
                    bool tieneStock = _context.StockItems
                        .Any(si => si.SucursalId == sucursal.Id && si.Cantidad > 0);

                    if (tieneStock)
                    {
                        ModelState.AddModelError("Activa",
                            "No se puede deshabilitar la sucursal porque existen stock items con cantidad mayor a 0.");

                        sucursal.Activa = true;

                        return View(sucursal);
                    }
                }

                try
                {
                    _context.Update(sucursal);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(sucursal);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
        // GET: Sucursales/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                // Use a delete-specific TempData key so Index only shows delete-related messages
                TempData["DeleteError"] = "Id inválido.";
                return RedirectToAction(nameof(Index));
            }

            var sucursal = _context.Sucursales
                                   .Include(s => s.StockItems)
                                   .FirstOrDefault(m => m.Id == id);
            if (sucursal == null)
            {
                TempData["DeleteError"] = "Sucursal no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // If there is any stock item with quantity > 0, prevent deletion and stay in Index (show message)
            if (sucursal.StockItems != null && sucursal.StockItems.Any(si => si.Cantidad > 0))
            {
                TempData["DeleteError"] = "No se puede eliminar la sucursal porque existen stock items con cantidad mayor a 0.";
                return RedirectToAction(nameof(Index));
            }

            return View(sucursal);
        }

		[Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(int id)
		{
			var sucursal = _context.Sucursales
								   .FirstOrDefault(s => s.Id == id);

			if (sucursal == null)
			{
				TempData["DeleteError"] = "Sucursal no encontrada.";
				return RedirectToAction(nameof(Index));
			}

			sucursal.Activa = false;

			try
			{
				_context.Update(sucursal);
				_context.SaveChanges();

				TempData["DeleteSuccess"] = "Sucursal deshabilitada correctamente.";
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateException)
			{
				TempData["DeleteError"] = "Error al deshabilitar la sucursal. Intente nuevamente.";
				return RedirectToAction(nameof(Index));
			}
		}

		private bool SucursalExists(int id)
        {
            return _context.Sucursales.Any(e => e.Id == id);
        }

        private List<Sucursal> GetSucursalsSeed()
        {
            return new List<Sucursal>
                {
                    new Sucursal
                    {
                        Nombre = "Sucursal Centro",
                        Direccion = "Av. Siempre Viva 742",
                        Email = "centro@ort.edu.ar",
                        Telefono = "+54 11 4000-1000",
                        Activa = true,
                        StockItems = new List<StockItem>(),
                        Compras = new List<Compra>()
                    },
                    new Sucursal
                    {
                        Nombre = "Sucursal Norte",
                        Direccion = "Panamericana Km 45",
                        Email = "norte@ort.edu.ar",
                        Telefono = "+54 11 4000-2000",
                        Activa = true,
                        StockItems = new List<StockItem>(),
                        Compras = new List<Compra>()
                    },
                    new Sucursal
                    {
                        Nombre = "Sucursal Sur",
                        Direccion = "Calle Falsa 123",
                        Email = "sur@ort.edu.ar",
                        Telefono = "+54 11 4000-3000",
                        Activa = false,
                        StockItems = new List<StockItem>(),
                        Compras = new List<Compra>()
                    }
                };
        }
    }
}
