using Carrito_B.Data;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carrito_B.Controllers
{
    public class CarritoItemsController : Controller
    {
        private readonly CarritoContext _context;
        private readonly UserManager<Persona> _userManager;
        public CarritoItemsController(CarritoContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CarritoItems
        public IActionResult Index()
        {
            var carritoContext = _context.CarritoItems.Include(c => c.Carrito).Include(c => c.Producto);
            return View( carritoContext.ToList());
        }

        // GET: CarritoItems/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.CarritoItems == null)
            {
                return NotFound();
            }

            var carritoItem =  _context.CarritoItems
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // GET: CarritoItems/Create
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion");
            return View();
        }

        // POST: CarritoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,CarritoId,ProductoId,Cantidad")] CarritoItem carritoItem)
        {
            
            var parentCarrito = _context.Carritos.Find(carritoItem.CarritoId);
            if (parentCarrito == null)
            {
                ModelState.AddModelError("CarritoId", "Carrito no encontrado.");
            }
            else if (!parentCarrito.Activo)
            {
               
                return Forbid();
            }

            var producto = _context.Productos.Find(carritoItem.ProductoId);
            if (producto == null)
            {
                ModelState.AddModelError("ProductoId", "Producto no encontrado.");
            }
            else
            {
                carritoItem.ValorUnitario = producto.PrecioVigente;
                ModelState.Remove(nameof(carritoItem.ValorUnitario));
            }

            if (ModelState.IsValid)
            {
                _context.CarritoItems.Add(carritoItem);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        [Authorize(Roles = Configs.ADMIN_ROLE)]

        // GET: CarritoItems/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.CarritoItems == null)
            {
                return NotFound();
            }

            var carritoItem = _context.CarritoItems.Find(id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            var parent = _context.Carritos.Find(carritoItem.CarritoId);
            if (parent != null && !parent.Activo)
            {
                
                return Forbid();
            }

            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // POST: CarritoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,CarritoId,ProductoId,Cantidad")] CarritoItem carritoItem)
        {
            if (id != carritoItem.Id)
            {
                return NotFound();
            }

            
            var parent = _context.Carritos.Find(carritoItem.CarritoId);
            if (parent != null && !parent.Activo)
            {
               
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.CarritoItems.Update(carritoItem);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoItemExists(carritoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // GET: CarritoItems/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.CarritoItems == null)
            {
                return NotFound();
            }

            var carritoItem = _context.CarritoItems
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefault(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            if (carritoItem.Carrito != null && !carritoItem.Carrito.Activo)
            {
                
                return Forbid();
            }

            return View(carritoItem);
        }

        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // POST: CarritoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var carritoItem = _context.CarritoItems.Find(id);
            if (carritoItem != null)
            {
                var parent = _context.Carritos.Find(carritoItem.CarritoId);
                if (parent != null && !parent.Activo)
                {
                    
                    return Forbid();
                }

                _context.CarritoItems.Remove(carritoItem);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("SumarCant")]
        [ValidateAntiForgeryToken]
        public IActionResult SumarCant(int id)
        {
            
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("IniciarSesion", "Account");

            var userIdStr = _userManager.GetUserId(User);
            if (!int.TryParse(userIdStr, out var userId))
                return Forbid();

            var carritoItem = _context.CarritoItems.Find(id);
            if (carritoItem == null) return NotFound();

            var parent = _context.Carritos.Find(carritoItem.CarritoId);
            if (parent == null) return NotFound();

            
            if (!parent.Activo) return Forbid();

            
            if (!User.IsInRole(Configs.ADMIN_ROLE) && parent.ClienteId != userId)
                return Forbid();

            
            var existeCompra = _context.Compras.Any(c => c.CarritoId == carritoItem.CarritoId);
            if (!existeCompra)
            {
                var producto = _context.Productos.Find(carritoItem.ProductoId);
                if (producto != null)
                {
                    carritoItem.ValorUnitario = producto.PrecioVigente;
                }

                carritoItem.Cantidad++;
                _context.Update(carritoItem);
                _context.SaveChanges();
            }

            return RedirectToAction("MiCarrito", "Carritos", new { id = parent.Id });
        }

        [HttpPost, ActionName("RestarCant")]
        [ValidateAntiForgeryToken]
        public IActionResult RestarCant(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("IniciarSesion", "Account");

            var userIdStr = _userManager.GetUserId(User);
            if (!int.TryParse(userIdStr, out var userId))
                return Forbid();

            var carritoItem = _context.CarritoItems.Find(id);
            if (carritoItem == null) return NotFound();

            var parent = _context.Carritos.Find(carritoItem.CarritoId);
            if (parent == null) return NotFound();

            if (!parent.Activo) return Forbid();

            if (!User.IsInRole(Configs.ADMIN_ROLE) && parent.ClienteId != userId)
                return Forbid();

            if (carritoItem.Cantidad > 1)
            {
                var existeCompra = _context.Compras.Any(c => c.CarritoId == carritoItem.CarritoId);
                if (!existeCompra)
                {
                    var producto = _context.Productos.Find(carritoItem.ProductoId);
                    if (producto != null)
                    {
                        carritoItem.ValorUnitario = producto.PrecioVigente;
                    }

                    carritoItem.Cantidad--;
                    _context.Update(carritoItem);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("MiCarrito", "Carritos", new { id = parent.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult EliminarDelCarrito(int id)
        {

            var userIdStr = _userManager.GetUserId(User);
            if (!int.TryParse(userIdStr, out var userId))
                return Forbid();

            var carritoItem = _context.CarritoItems
                .Include(ci => ci.Carrito)
                .FirstOrDefault(ci => ci.Id == id);

            if (carritoItem == null)
                return NotFound();

            var carrito = carritoItem.Carrito;
            if (carrito == null || !carrito.Activo)
                return Forbid();

            if (!User.IsInRole(Configs.ADMIN_ROLE) && carrito.ClienteId != userId)
                return Forbid();

            var existeCompra = _context.Compras.Any(c => c.CarritoId == carrito.Id);
            if (!existeCompra)
            {
                _context.CarritoItems.Remove(carritoItem);
                _context.SaveChanges();
            }

            return RedirectToAction("MiCarrito", "Carritos");
        }

        private bool CarritoItemExists(int id)
        {
            return _context.CarritoItems.Any(e => e.Id == id);
        }
    }
}
