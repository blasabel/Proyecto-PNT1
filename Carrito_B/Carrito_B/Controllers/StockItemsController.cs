using Carrito_B.Data;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carrito_B.Controllers
{
    [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
    public class StockItemsController : Controller
    {
        private readonly CarritoContext _context;

        public StockItemsController(CarritoContext context)
        {
            _context = context;
        }
        // GET: StockItems
        public async Task<IActionResult> Index(int? sucursalId)
        {
            var sucursales = await _context.Sucursales
                .Where(s => s.Activa)
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            ViewData["Sucursales"] = new SelectList(sucursales, "Id", "Nombre", sucursalId);

            var query = _context.StockItems
                .Include(si => si.Producto)
                .Include(si => si.Sucursal)
                .AsQueryable();

            if (sucursalId.HasValue)
            {
                ViewBag.SucursalId = sucursalId.Value;
                query = query.Where(si => si.SucursalId == sucursalId.Value);
            }
            else
            {
                ViewBag.SucursalId = null;
            }

            var lista = await query.ToListAsync();
            return View(lista);
        }

        // GET: StockItems/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var stockItem = _context.StockItems
                                    .Include(s => s.Producto)
                                    .Include(s => s.Sucursal)
                                    .FirstOrDefault(m => m.Id == id);
            if (stockItem == null) return NotFound();

            return View(stockItem);
        }

        // GET: StockItems/Create
        public IActionResult Create(int sucursalId)
        {
            var sucursal = _context.Sucursales.FirstOrDefault(s => s.Id == sucursalId);
            if (sucursal == null) return NotFound();

            var productosEnStock = _context.StockItems
                .Where(si => si.SucursalId == sucursalId)
                .Select(si => si.ProductoId)
                .ToList();

            var productosDisponibles = _context.Productos
                .Where(p => p.Activo && !productosEnStock.Contains(p.Id))
                .OrderBy(p => p.Descripcion)
                .ToList();

            ViewBag.Sucursal = sucursal;
            ViewData["ProductoId"] = new SelectList(productosDisponibles, "Id", "Descripcion");

            var model = new StockItem
            {
                SucursalId = sucursalId,
                Cantidad = 0
            };

            return View(model);
        }

        // POST: StockItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("SucursalId,ProductoId,Cantidad")] StockItem stockItem)
        {
            if (stockItem.Cantidad <= 0)
            {
                ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0.");
            }

            var yaExiste = _context.StockItems
                .Any(s => s.SucursalId == stockItem.SucursalId && s.ProductoId == stockItem.ProductoId);

            if (yaExiste)
            {
                ModelState.AddModelError("ProductoId", "Ya existe stock para ese producto en esa sucursal.");
            }

            if (!ModelState.IsValid)
            {
                var sucursal = _context.Sucursales.FirstOrDefault(s => s.Id == stockItem.SucursalId);

                var productosEnStock = _context.StockItems
                    .Where(si => si.SucursalId == stockItem.SucursalId)
                    .Select(si => si.ProductoId)
                    .ToList();

                var productosDisponibles = _context.Productos
                    .Where(p => p.Activo && !productosEnStock.Contains(p.Id))
                    .OrderBy(p => p.Descripcion)
                    .ToList();

                ViewBag.Sucursal = sucursal;
                ViewData["ProductoId"] = new SelectList(productosDisponibles, "Id", "Descripcion", stockItem.ProductoId);

                return View(stockItem);
            }

            _context.StockItems.Add(stockItem);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { sucursalId = stockItem.SucursalId });
        }

        // GET: StockItems/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var stockItem = _context.StockItems.Find(id);
            if (stockItem == null) return NotFound();

            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", stockItem.ProductoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Direccion", stockItem.SucursalId);
            return View(stockItem);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // POST: StockItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,SucursalId,ProductoId,Cantidad")] StockItem stockItem)
        {
            if (id != stockItem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockItem);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockItemExists(stockItem.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", stockItem.ProductoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Direccion", stockItem.SucursalId);
            return View(stockItem);
        }

        // GET: StockItems/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var stockItem = _context.StockItems
                                    .Include(s => s.Producto)
                                    .Include(s => s.Sucursal)
                                    .FirstOrDefault(m => m.Id == id);
            if (stockItem == null) return NotFound();

            return View(stockItem);
        }

        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // POST: StockItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var stockItem = _context.StockItems.Find(id);
            if (stockItem != null)
            {
                _context.StockItems.Remove(stockItem);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: StockItems/SumarStock/5
        public IActionResult SumarStock(int? id, int? sucursalId)
        {
            if (id == null) return NotFound();

            var stockItem = _context.StockItems
                                    .Include(s => s.Producto)
                                    .Include(s => s.Sucursal)
                                    .FirstOrDefault(s => s.Id == id);
            if (stockItem == null) return NotFound();

            ViewBag.SucursalFiltroId = sucursalId;

            return View("SumarStock", stockItem);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // POST: StockItems/SumarStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SumarStock(int id, int? cantidad, int? sucursalFiltroId)
        {
            var stockItem = _context.StockItems
                                    .Include(s => s.Producto)
                                    .Include(s => s.Sucursal)
                                    .FirstOrDefault(s => s.Id == id);
            if (stockItem == null) return NotFound();

            if (!cantidad.HasValue || cantidad.Value <= 0)
            {
                ModelState.AddModelError(nameof(cantidad), "La cantidad a sumar debe ser mayor que 0.");
                ViewBag.SucursalFiltroId = sucursalFiltroId;
                return View("SumarStock", stockItem);
            }

            try
            {
                stockItem.Cantidad += cantidad.Value;
                _context.Update(stockItem);
                _context.SaveChanges();

                TempData["Success"] = $"Se agregaron {cantidad.Value} unidades a \"{stockItem.Producto?.Nombre ?? $"Producto {stockItem.ProductoId}"}\" en {stockItem.Sucursal?.Nombre}.";
                
                if (sucursalFiltroId.HasValue)
                {
                    return RedirectToAction(nameof(Index), new { sucursalId = sucursalFiltroId.Value });
                }
                
                return RedirectToAction(nameof(Index), new { sucursalId = stockItem.SucursalId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar la base de datos. Intente nuevamente.");
                return View("SumarStock", stockItem);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado. Intente nuevamente.");
                return View("SumarStock", stockItem);
            }
        }

        private bool StockItemExists(int id)
        {
            return _context.StockItems.Any(e => e.Id == id);
        }
    }
}
