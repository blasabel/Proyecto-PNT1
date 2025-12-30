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
    public class CarritosController : Controller
    {
        private readonly CarritoContext _context;
        private readonly SignInManager<Persona> _signInManager;
        private readonly UserManager<Persona> _userManager;

        public CarritosController(
            CarritoContext context,
            SignInManager<Persona> signInManager,
            UserManager<Persona> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.CLIENTE_ROLE}")]
        // GET: Carritos
        public IActionResult Index()
        {
           
            if (User.IsInRole(Configs.CLIENTE_ROLE))
            {
                var userIdString = _userManager.GetUserId(User);
                if (int.TryParse(userIdString, out var userId))
                {
                    var carritoContext = _context.Carritos
                                                .Include(c => c.Cliente)
                                                .Where(c => c.ClienteId == userId);
                    return View(carritoContext.ToList());
                }
            }

            
            var allCarritos = _context.Carritos.Include(c => c.Cliente);
            return View(allCarritos.ToList());
        }


        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // GET: Carritos/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Carritos == null)
            {
                return NotFound();
            }

            var carrito = _context.Carritos
                .Include(c => c.Cliente)
                .Include(ci => ci.CarritoItems)
                .ThenInclude(p => p.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritos/Edit/5
        [Authorize(Roles = $"{Configs.CLIENTE_ROLE}")]
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Carritos == null)
            {
                return NotFound();
            }

            var carrito = _context.Carritos.Find(id);
            if (carrito == null)
            {
                return NotFound();
            }

            
            if (!carrito.Activo)
            {
                return Forbid();
            }

            
            if (!esClientePropietario(carrito))
            {
                return Forbid();
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        [Authorize(Roles = $"{Configs.CLIENTE_ROLE}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Activo,ClienteId")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            var existing = _context.Carritos.Find(id);
            if (existing == null)
            {
                return NotFound();
            }

            
            if (!existing.Activo)
            {
                return Forbid();
            }

            
            if (!esClientePropietario(existing))
            {
                return Forbid();
            }

            
            carrito.Activo = existing.Activo;
            carrito.ClienteId = existing.ClienteId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Carritos.Update(carrito);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        [Authorize]
        public IActionResult Delete(int? id)
        {
            
            TempData["Error"] = "Los carritos no pueden eliminarse.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Carritos/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
           
            TempData["Error"] = "Los carritos no pueden eliminarse.";
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(int id)
        {
            return _context.Carritos.Any(e => e.Id == id);
        }


        [Authorize(Roles = $"{Configs.CLIENTE_ROLE}")]
        [HttpPost, ActionName("Vaciar")]
        [ValidateAntiForgeryToken]

        public IActionResult VaciarCarrito(int id)
        {
            var carrito = _context.Carritos
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            
            if (!carrito.Activo)
            {
                return Forbid();
            }

            
            if (!esClientePropietario(carrito))
            {
                return Forbid();
            }

            _context.CarritoItems.RemoveRange(carrito.CarritoItems);
            _context.SaveChanges();
            return RedirectToAction(nameof(MiCarrito), new { id = carrito.Id });
        }


        
        [HttpPost,ActionName("AddCarrito")]
        [ValidateAntiForgeryToken]
        public IActionResult AddCarrito(int productoId, int cantidad)
		{

            TempData.Remove("PendingProductoId");
            TempData.Remove("PendingCantidad");

            if (!_signInManager.IsSignedIn(User))
            {
                TempData["PendingProductoId"] = productoId;
                TempData["PendingCantidad"] = cantidad;

				return RedirectToAction("IniciarSesion", "Account");
            }

            if (!User.IsInRole(Configs.CLIENTE_ROLE))
            {
                return Forbid();
            }

            int userId = int.Parse(_userManager.GetUserId(User));

			var carritoUser = AgregarProductoAlCarrito(userId, productoId, cantidad);

			return RedirectToAction(nameof(MiCarrito), new { id = carritoUser.Id });
		}

        
        private Carrito AgregarProductoAlCarrito(int userId, int productoId, int cantidad)
		{
			var producto = _context.Productos.Find(productoId);
			if (producto == null)
				throw new InvalidOperationException("Producto no encontrado");

			var carritoUser = _context.Carritos
				.Include(c => c.CarritoItems)
				.FirstOrDefault(c => c.ClienteId == userId && c.Activo);

			if (carritoUser == null)
			{
				carritoUser = new Carrito
				{
					Activo = true,
					ClienteId = userId,
					CarritoItems = new List<CarritoItem>()
				};
				_context.Carritos.Add(carritoUser);
				_context.SaveChanges();
			}

			if (NoExisteCarritoItem(producto.Id, carritoUser))
			{
				var item = new CarritoItem
				{
					ProductoId = producto.Id,
					Producto = producto,
					Carrito = carritoUser,
					CarritoId = carritoUser.Id,
					Cantidad = cantidad,
					ValorUnitario = producto.PrecioVigente
				};
				carritoUser.CarritoItems.Add(item);
				_context.SaveChanges();
			} else {
				var productoRepetido = carritoUser.CarritoItems
					.FirstOrDefault(p => p.ProductoId == producto.Id);

				if (productoRepetido != null)
				{
					productoRepetido.Cantidad += cantidad;
					_context.SaveChanges();
				}
			}

			return carritoUser;
		}

        [Authorize(Roles = Configs.CLIENTE_ROLE)]
        public IActionResult CompletarAddCarritoPostLogin()
		{
			if (!TempData.ContainsKey("PendingProductoId"))
			{
                TempData.Remove("PendingCantidad");
                return RedirectToAction("Index", "Productos");
			}

			int productoId = int.Parse(TempData["PendingProductoId"].ToString());
			int cantidad = int.Parse((TempData["PendingCantidad"] ?? "1").ToString());

			int userId = int.Parse(_userManager.GetUserId(User));

            TempData.Remove("PendingProductoId");
            TempData.Remove("PendingCantidad");

            var carritoUser = AgregarProductoAlCarrito(userId, productoId, cantidad);

			return RedirectToAction(nameof(MiCarrito), new { id = carritoUser.Id });
		}

		[HttpGet]
        [Authorize(Roles = $"{Configs.CLIENTE_ROLE}")]
        public IActionResult MiCarrito()
        {

            var userIdString = _userManager.GetUserId(User);
            if (!int.TryParse(userIdString, out var userId))
                return Forbid();

            var carrito = _context.Carritos
                .Include(c => c.CarritoItems)
                    .ThenInclude(ci => ci.Producto)
                .FirstOrDefault(c => c.ClienteId == userId && c.Activo);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    Id = 0,
                    Activo = true,
                    ClienteId = userId,
                    CarritoItems = new List<CarritoItem>()
                };
            }

            return View("MiCarrito", carrito);
        }

        private bool NoExisteCarritoItem(int productoId, Carrito carrito)
        {
            if (carrito.CarritoItems != null)
            {
                return !carrito.CarritoItems.Any(ci => ci.ProductoId == productoId);
            }

            return !_context.CarritoItems.Any(ci => ci.ProductoId == productoId && ci.CarritoId == carrito.Id);
        }

        
        private bool esClientePropietario(Carrito carrito)
        {
            if (carrito == null) return false;

            var userIdString = _userManager.GetUserId(User);
            if (!int.TryParse(userIdString, out var userId)) return false;

            return carrito.ClienteId == userId;
        }
    }
}
