using Carrito_B.Data;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace Carrito_B.Controllers
{

    public class ProductosController : Controller
    {
        private readonly CarritoContext _context;
        private readonly UserManager<Persona> _userManager;

        public ProductosController(CarritoContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Productos
        public IActionResult Index(int? categoriaId)
        {

            ViewData["Categorias"] = new SelectList(_context.Categorias.ToList(), "Id", "Nombre", categoriaId);

           
            var productos = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.StockItems)
                .AsQueryable();

           
            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId.Value);
            }

            // 4️⃣ Devuelvo la lista (filtrada o no)
            return View(productos.ToList());
        }
        

        // GET: Productos/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.StockItems)
                    .ThenInclude(si => si.Sucursal)
                .FirstOrDefault(p => p.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // GET: Productos/Create
        public IActionResult Create()
        {

            EnsureSeedCategoriasYMarcas();
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Id,Activo,Nombre,Descripcion,PrecioVigente,CategoriaId,Imagen,MarcaId")]
            Producto producto,
            IFormFile? ImagenFile)
        {
            producto.Nombre = producto.Nombre?.Trim();
            producto.Descripcion = producto.Descripcion?.Trim();
            producto.Imagen = producto.Imagen?.Trim();

            if ((ImagenFile == null || ImagenFile.Length == 0) && string.IsNullOrWhiteSpace(producto.Imagen))
            {
                ModelState.AddModelError("Imagen", "Ingresá una URL o subí un archivo.");
            }

            bool existe = _context.Productos.Any(p =>
                p.MarcaId == producto.MarcaId &&
                p.Nombre == producto.Nombre);
            if (existe)
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con ese nombre para esa marca.");
            }

            if (!ModelState.IsValid)
            {
                CargarListas(producto.CategoriaId, producto.MarcaId);
                return View(producto);
            }

            if (ImagenFile is { Length: > 0 })
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!allowed.Contains(ImagenFile.ContentType))
                {
                    ModelState.AddModelError("Imagen", "Formato no soportado (solo jpg, png, gif, webp).");
                    CargarListas(producto.CategoriaId, producto.MarcaId);
                    return View(producto);
                }

                using var ms = new MemoryStream();
                ImagenFile.CopyTo(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                producto.Imagen = $"data:{ImagenFile.ContentType};base64,{base64}";
            }

            try
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql &&
                                               (sql.Number == 2601 || sql.Number == 2627))
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con ese nombre para esa marca.");
                CargarListas(producto.CategoriaId, producto.MarcaId);
                return View(producto);
            }
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // GET: Productos/Edit/5
        public IActionResult Edit(int? id)
        {

            EnsureSeedCategoriasYMarcas();

            if (id == null) return NotFound();

            var producto = _context.Productos.Find(id);
            if (producto == null) return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", producto.MarcaId);
            return View(producto);
        }

		[Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
		// POST: Productos/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(
	int id,
	[Bind("Id,Activo,Nombre,Descripcion,PrecioVigente,CategoriaId,Imagen,MarcaId")]
	Producto producto,
	IFormFile? ImagenFile)
		{
			if (id != producto.Id) return NotFound();

			producto.Nombre = producto.Nombre?.Trim();
			producto.Descripcion = producto.Descripcion?.Trim();
			producto.Imagen = producto.Imagen?.Trim();

			if (!ModelState.IsValid)
			{
				CargarListas(producto.CategoriaId, producto.MarcaId);
				return View(producto);
			}

			var db = _context.Productos.FirstOrDefault(p => p.Id == id);
			if (db == null) return NotFound();

			bool existe = _context.Productos.Any(p =>
				p.Id != producto.Id &&
				p.MarcaId == producto.MarcaId &&
				p.Nombre == producto.Nombre);
			if (existe)
			{
				ModelState.AddModelError("Nombre", "Ya existe un producto con ese nombre para esa marca.");
				CargarListas(producto.CategoriaId, producto.MarcaId);
				return View(producto);
			}

			var precioAnterior = db.PrecioVigente;

			db.Activo = producto.Activo;
			db.Nombre = producto.Nombre;
			db.Descripcion = producto.Descripcion;
			db.PrecioVigente = producto.PrecioVigente;
			db.CategoriaId = producto.CategoriaId;
			db.MarcaId = producto.MarcaId;

			if (ImagenFile is { Length: > 0 })
			{
				var allowed = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
				if (!allowed.Contains(ImagenFile.ContentType))
				{
					ModelState.AddModelError("Imagen", "Formato no soportado (solo jpg, png, gif, webp).");
					CargarListas(producto.CategoriaId, producto.MarcaId);
					return View(producto);
				}

				using var ms = new MemoryStream();
				ImagenFile.CopyTo(ms);
				var base64 = Convert.ToBase64String(ms.ToArray());
				db.Imagen = $"data:{ImagenFile.ContentType};base64,{base64}";
			}
			else
			{
				db.Imagen = string.IsNullOrWhiteSpace(producto.Imagen) ? db.Imagen : producto.Imagen;
			}

			try
			{
				_context.Update(db);
				_context.SaveChanges();

				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateException ex) when (ex.InnerException is SqlException sql &&
											   (sql.Number == 2601 || sql.Number == 2627))
			{
				ModelState.AddModelError("Nombre", "Ya existe un producto con ese nombre para esa marca.");
				CargarListas(producto.CategoriaId, producto.MarcaId);
				return View(producto);
			}
		}

		[Authorize(Roles = Configs.ADMIN_ROLE)]
        // GET: Productos/Delete/5
        public IActionResult Delete(int? id) => NotFound();

        [Authorize(Roles = Configs.ADMIN_ROLE)]
        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) => NotFound();

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        [HttpPost,ActionName("DeactProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult DeactProduct(int id)
        {
            var producto = _context.Productos.Find(id);
            if(producto != null)
            {
                producto.Activo = false;
            }

            _context.Update(producto);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        [HttpPost, ActionName("ActProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult ActProduct(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto != null)
            {
                producto.Activo = true;
            }
            _context.Update(producto);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
            => _context.Productos.Any(e => e.Id == id);

        private void EnsureSeedCategoriasYMarcas()
        {
            // Categorías
            if (!_context.Categorias.Any())
            {
                _context.Categorias.AddRange(
                    new Categoria { Nombre = "Bebidas", Descripcion = "Bebidas y refrescos" },
                    new Categoria { Nombre = "Alimentos", Descripcion = "Comestibles en general" },
                    new Categoria { Nombre = "Limpieza", Descripcion = "Productos de limpieza" }
                );
            }

            // Marcas
            if (!_context.Marcas.Any())
            {
                _context.Marcas.AddRange(
                    new Marca { Nombre = "Coca-Cola", Descripcion = "Bebidas gaseosas" },
                    new Marca { Nombre = "Pepsi", Descripcion = "Bebidas gaseosas" },
                    new Marca { Nombre = "Acme", Descripcion = "Marca genérica" }
                );
            }

            // Solo hago SaveChanges si agregué algo
            if (_context.ChangeTracker.HasChanges())
                _context.SaveChanges();
        }

        private void CargarListas(int? categoriaId = null, int? marcaId = null)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", categoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", marcaId);
        }
    }


}
