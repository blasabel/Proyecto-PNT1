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

namespace Carrito_B.Controllers
{

    public class ComprasController : Controller
    {
        private readonly CarritoContext _context;
        private readonly UserManager<Persona> _userManager;

        public ComprasController(CarritoContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
        // GET: Compras
        public IActionResult Index(int? mes, int? anio, int? clienteId)
        {
            var compras = _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Sucursal)
                .Include(c => c.Cliente)
                .AsQueryable();

            if (clienteId.HasValue)
            {
                compras = compras.Where(c => c.Cliente.Id == clienteId.Value);
            }

            if (anio.HasValue)
            {
                compras = compras.Where(c => c.Fecha.Year == anio.Value);
            }

            if (mes.HasValue)
            {
                compras = compras.Where(c => c.Fecha.Month == mes.Value);
            }

            compras = compras.OrderByDescending(c => c.Total);

            if (clienteId.HasValue)
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.Id == clienteId.Value);
                if (cliente != null)
                {
                    ViewBag.ClienteNombre = $"{cliente.Nombre} {cliente.Apellido}";
                }
            }

            ViewBag.Mes = mes;
            ViewBag.Anio = anio;
            ViewBag.ClienteId = clienteId;

            return View(compras.ToList());
        }


        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
        public IActionResult PorCliente(int id, int? mes, int? anio)
        {
            var compras = _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Sucursal)
                .Include(c => c.Cliente)
                .Where(c => c.Cliente.Id == id)
                .AsQueryable();

            if (anio.HasValue)
            {
                compras = compras.Where(c => c.Fecha.Year == anio.Value);
            }

            if (mes.HasValue)
            {
                compras = compras.Where(c => c.Fecha.Month == mes.Value);
            }

            compras = compras.OrderByDescending(c => c.Total);

            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);

            ViewBag.Mes = mes;
            ViewBag.Anio = anio;
            ViewBag.ClienteId = id;
            ViewBag.ClienteNombre = cliente != null
                ? $"{cliente.Nombre} {cliente.Apellido}"
                : $"Id {id}";

            return View("Index", compras.ToList());
        }

        [Authorize]
                    // GET: Compras/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = _context.Compras
                .Include(c => c.Carrito)
                    .ThenInclude(carrito => carrito.CarritoItems)
                        .ThenInclude(item => item.Producto)
                .Include(c => c.Sucursal)
                .Include(c => c.Cliente)
                .FirstOrDefault(m => m.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            if (User.IsInRole(Configs.ADMIN_ROLE) || User.IsInRole(Configs.EMPLEADO_ROLE))
            {
                return View(compra);
            }

            if (User.IsInRole(Configs.CLIENTE_ROLE))
            {
                int userId = Int32.Parse(_userManager.GetUserId(User));

                if (compra.Cliente.Id != userId)
                {
                    return Forbid();
                }

                return View(compra);
            }

            return Forbid();
        }



        [Authorize(Roles = $"{Configs.CLIENTE_ROLE}")]
        // GET: Compras/Create
        public IActionResult Create(int? carritoId)
        {
            ViewData["CarritoId"] = new SelectList(
                _context.Carritos,
                "Id",
                "Id",
                carritoId
            );

            ViewData["SucursalId"] = new SelectList(
                _context.Sucursales
                    .Where(s => s.Activa)
                    .Select(s => new {
                        s.Id,
                        Texto = s.Nombre + " - " + s.Direccion
                    }),
                "Id",
                "Texto"
            );

            return View();
        }
        // POST: Compras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create([Bind("Id,SucursalId")] Compra compra)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                ViewData["SucursalId"] = new SelectList(
                    _context.Sucursales.Where(s => s.Activa),
                    "Id",
                    "Direccion",
                    compra.SucursalId
                );
                return View(compra);
            }

            int userId = Int32.Parse(_userManager.GetUserId(User));

            var cliente = _context.Clientes
                .Where(c => c.Id == userId)
                .First();

            // Buscar el carrito ACTIVO del cliente (evita tomar carritos históricos)
            var carrito = _context.Carritos
                .Include(c => c.CarritoItems)
                    .ThenInclude(i => i.Producto)
                .Where(c => c.ClienteId == userId && c.Activo)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            if (carrito == null)
            {
                ModelState.AddModelError("", "No se encontró un carrito activo para el usuario.");
                ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                ViewData["SucursalId"] = new SelectList(
                    _context.Sucursales.Where(s => s.Activa),
                    "Id",
                    "Direccion",
                    compra.SucursalId
                );
                return View(compra);
            }

            // ahora que carrito != null, asignar con seguridad
            compra.CarritoId = carrito.Id;

            if (carrito.CarritoItems == null || !carrito.CarritoItems.Any())
            {
                ModelState.AddModelError("", "El carrito seleccionado no contiene ítems. No se puede efectuar la compra.");
                ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                ViewData["SucursalId"] = new SelectList(
                    _context.Sucursales.Where(s => s.Activa),
                    "Id",
                    "Direccion",
                    compra.SucursalId
                );
                return View(compra);
            }

            var sucursal = _context.Sucursales
                .Include(s => s.StockItems)
                .FirstOrDefault(s => s.Id == compra.SucursalId && s.Activa);

            if (sucursal == null)
            {
                ModelState.AddModelError("", "La sucursal seleccionada no está disponible.");
                ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                ViewData["SucursalId"] = new SelectList(
                    _context.Sucursales.Where(s => s.Activa),
                    "Id",
                    "Direccion",
                    compra.SucursalId
                );
                return View(compra);
            }

            var faltantes = new List<string>();
            foreach (var item in carrito.CarritoItems)
            {
                var stock = sucursal.StockItems.FirstOrDefault(sp => sp.ProductoId == item.ProductoId);
                if (stock == null || stock.Cantidad < item.Cantidad)
                    faltantes.Add(item.Producto?.Nombre ?? $"Producto {item.ProductoId}");
            }

            if (faltantes.Any())
            {
                var sucursalesAlternativas = _context.Sucursales
                    .Include(s => s.StockItems)
                    .Where(s => s.Id != sucursal.Id && s.Activa)
                    .ToList()
                    .Where(s => carrito.CarritoItems.All(item =>
                        s.StockItems.Any(sp => sp.ProductoId == item.ProductoId && sp.Cantidad >= item.Cantidad)))
                    .ToList();

                if (!sucursalesAlternativas.Any())
                {
                    ModelState.AddModelError("", "No hay stock disponible en ninguna sucursal para los productos seleccionados.");
                    ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                    ViewData["SucursalId"] = new SelectList(
                        _context.Sucursales.Where(s => s.Activa),
                        "Id",
                        "Direccion",
                        compra.SucursalId
                    );
                    ViewBag.Faltantes = faltantes;
                    return View(compra);
                }

                ViewBag.SucursalesAlternativas = sucursalesAlternativas;
                ViewBag.Faltantes = faltantes;
                return View("SeleccionarSucursal", compra);
            }

            // Operación en transacción: restar stock, crear compra, desactivar carrito viejo y crear uno nuevo activo
            using var tx = _context.Database.BeginTransaction();
            try
            {
                foreach (var item in carrito.CarritoItems)
                {
                    var stock = sucursal.StockItems.First(sp => sp.ProductoId == item.ProductoId);
                    if (stock.Cantidad < item.Cantidad)
                    {
                        tx.Rollback();
                        ModelState.AddModelError("", "Stock insuficiente al intentar efectuar la compra. Intente nuevamente.");
                        ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                        ViewData["SucursalId"] = new SelectList(
                            _context.Sucursales.Where(s => s.Activa),
                            "Id",
                            "Direccion",
                            compra.SucursalId
                        );
                        return View(compra);
                    }
                    stock.Cantidad -= item.Cantidad;
                }
                compra.Cliente = cliente;
                compra.Fecha = DateTime.Now;
                compra.Total = CalcularTotal(carrito.Id);

                if (compra.Total <= 0)
                {
                    tx.Rollback();
                    ModelState.AddModelError("", "El total de la compra es cero. No se puede efectuar la compra.");
                    ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                    ViewData["SucursalId"] = new SelectList(
                        _context.Sucursales.Where(s => s.Activa),
                        "Id",
                        "Direccion",
                        compra.SucursalId
                    );
                    return View(compra);
                }

                // Verificar que no exista ya una compra para este carrito por seguridad
                if (_context.Compras.Any(c => c.CarritoId == compra.CarritoId))
                {
                    tx.Rollback();
                    ModelState.AddModelError("", "Este carrito ya tiene una compra registrada.");
                    ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                    ViewData["SucursalId"] = new SelectList(
                        _context.Sucursales.Where(s => s.Activa),
                        "Id",
                        "Direccion",
                        compra.SucursalId
                    );
                    return View(compra);
                }

                carrito.Activo = false;
                _context.Compras.Add(compra);

                var nuevoCarrito = new Carrito
                {
                    Activo = true,
                    ClienteId = carrito.ClienteId
                };
                _context.Carritos.Add(nuevoCarrito);

                _context.SaveChanges();
                tx.Commit();

                return RedirectToAction(nameof(Gracias), new { id = compra.Id });
            }
            catch
            {
                tx.Rollback();
                ModelState.AddModelError("", "Ocurrió un error al procesar la compra. Intente nuevamente.");
                ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
                ViewData["SucursalId"] = new SelectList(
                    _context.Sucursales.Where(s => s.Activa),
                    "Id",
                    "Direccion",
                    compra.SucursalId
                );
                return View(compra);
            }
        }

        [Authorize(Roles = $"{Configs.CLIENTE_ROLE}")]
        // GET: Compras/Gracias/5
        [Authorize(Roles = Configs.CLIENTE_ROLE)]
        public IActionResult Gracias(int id)
        {
            var compra = _context.Compras
                .Include(c => c.Sucursal)
                .FirstOrDefault(c => c.Id == id);

            if (compra == null) return NotFound();


            var carritoItems = _context.CarritoItems
                .Include(ci => ci.Producto)
                .Where(ci => ci.CarritoId == compra.CarritoId)
                .ToList();


            compra.Carrito = _context.Carritos.Find(compra.CarritoId) ?? new Carrito { Id = compra.CarritoId };
            compra.Carrito.CarritoItems = carritoItems;

            return View(compra);
        }

        // POST: Compras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,CarritoId,SucursalId,Fecha,Total")] Compra compra)
        {
            if (id != compra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Direccion", compra.SucursalId);
            return View(compra);
        }

        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // GET: Compras/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var compra = _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Sucursal)
                .FirstOrDefault(m => m.Id == id);

            if (compra == null) return NotFound();
            return View(compra);
        }

        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var compra = _context.Compras.Find(id);
            if (compra != null)
            {
                _context.Compras.Remove(compra);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }

        [HttpPost]

        public IActionResult ConfirmarSucursal(int CarritoId, int SucursalId)
        {

            var carrito = _context.Carritos
                .Include(c => c.CarritoItems)
                    .ThenInclude(ci => ci.Producto)
                .FirstOrDefault(c => c.Id == CarritoId);

            if (carrito == null || carrito.CarritoItems == null || !carrito.CarritoItems.Any())
            {
                TempData["Error"] = "El carrito no contiene ítems. No se puede efectuar la compra.";
                return RedirectToAction("Create");
            }


            var sucursal = _context.Sucursales
                .Include(s => s.StockItems)
                .FirstOrDefault(s => s.Id == SucursalId);

            if (sucursal == null)
            {
                TempData["Error"] = "Sucursal no encontrada.";
                return RedirectToAction("Create");
            }


            var faltantes = new List<string>();
            foreach (var item in carrito.CarritoItems)
            {
                var stock = sucursal.StockItems.FirstOrDefault(sp => sp.ProductoId == item.ProductoId);
                if (stock == null || stock.Cantidad < item.Cantidad)
                    faltantes.Add(item.Producto?.Nombre ?? $"Producto {item.ProductoId}");
            }

            if (faltantes.Any())
            {

                var sucursalesAlternativas = _context.Sucursales
                    .Include(s => s.StockItems)
                    .Where(s => s.Id != sucursal.Id)
                    .ToList()
                    .Where(s => carrito.CarritoItems.All(item =>
                        s.StockItems.Any(sp => sp.ProductoId == item.ProductoId && sp.Cantidad >= item.Cantidad)))
                    .ToList();

                if (sucursalesAlternativas.Any())
                {
                    ViewBag.SucursalesAlternativas = sucursalesAlternativas;
                    ViewBag.Faltantes = faltantes;
                    var compraModel = new Compra { CarritoId = CarritoId, SucursalId = SucursalId };
                    return View("SeleccionarSucursal", compraModel);
                }
                else
                {
                    TempData["Error"] = "No hay stock disponible en ninguna sucursal para los productos seleccionados.";
                    return RedirectToAction("Create");
                }
            }

            
            var cliente = _context.Clientes
                .Where(c => c.Id == carrito.ClienteId)
                .First();

            using var tx = _context.Database.BeginTransaction();
            try
            {

                foreach (var item in carrito.CarritoItems)
                {
                    var stock = sucursal.StockItems.First(sp => sp.ProductoId == item.ProductoId);
                    if (stock.Cantidad < item.Cantidad)
                    {
                        tx.Rollback();
                        TempData["Error"] = "Stock insuficiente al intentar confirmar la compra. Intente nuevamente.";
                        return RedirectToAction("Create");
                    }
                    stock.Cantidad -= item.Cantidad;
                }

                var compra = new Compra
                {
                    CarritoId = CarritoId,
                    Cliente = cliente,
                    SucursalId = SucursalId,
                    Fecha = DateTime.Now,
                    Total = CalcularTotal(CarritoId)
                };

                if (compra.Total <= 0)
                {
                    tx.Rollback();
                    TempData["Error"] = "El total de la compra es cero. No se puede efectuar la compra.";
                    return RedirectToAction("Create");
                }


                carrito.Activo = false;

                // Crear nuevo carrito activo para el cliente dentro de la transacción
                var nuevoCarrito = new Carrito
                {
                    Activo = true,
                    ClienteId = carrito.ClienteId,
                    CarritoItems = new List<CarritoItem>()
                };

                _context.Compras.Add(compra);
                _context.Carritos.Add(nuevoCarrito);
                _context.SaveChanges();

                tx.Commit();

                return RedirectToAction(nameof(Gracias), new { id = compra.Id });
            }
            catch
            {
                tx.Rollback();
                TempData["Error"] = "Ocurrió un error al confirmar la compra. Intente nuevamente.";
                return RedirectToAction("Create");
            }
        }

        public IActionResult CancelarCompra(int carritoId)
        {
            var carrito = _context.Carritos
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Id == carritoId);

            if (carrito != null && carrito.CarritoItems.Any())
            {
                _context.CarritoItems.RemoveRange(carrito.CarritoItems);
                _context.SaveChanges();
            }

            TempData["Warning"] = "Compra cancelada y carrito vaciado.";
            return RedirectToAction("MiCarrito", "Carritos");
        }



        private decimal CalcularTotal(int carritoId)
        {
            var carrito = _context.Carritos
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Id == carritoId);

            if (carrito == null || !carrito.CarritoItems.Any())
                return 0;

            return carrito.CarritoItems.Sum(item => item.Cantidad * item.ValorUnitario);
        }

    }
}
