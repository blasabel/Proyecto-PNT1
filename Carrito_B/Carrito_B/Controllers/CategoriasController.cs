using Carrito_B.Data;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carrito_B.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly CarritoContext _context;

        public CategoriasController(CarritoContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public IActionResult Index()
        {
            return View(_context.Categorias.ToList());
        }

        // GET: Categorias/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = _context.Categorias
                .FirstOrDefault(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            if (_context.Categorias.Any(s => s.Nombre.ToLower() == categoria.Nombre.ToLower()))
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoria con ese nombre.");
                return View(categoria);
            }

            if (ModelState.IsValid)
            {
                _context.Categorias.Add(categoria);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

        // GET: Categorias/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();

            categoria.Nombre = categoria.Nombre?.Trim();

            bool existeConMismoNombre = _context.Categorias
                .Any(c => c.Id != categoria.Id &&
                          c.Nombre.ToLower() == categoria.Nombre.ToLower());

            if (existeConMismoNombre)
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoría con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var CategoriaEnDB = _context.Categorias.Find(categoria.Id);
                    if (CategoriaEnDB != null)
                    {
                        CategoriaEnDB.Nombre = categoria.Nombre;
                        CategoriaEnDB.Descripcion = categoria.Descripcion;
                        _context.Categorias.Update(CategoriaEnDB);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return NotFound();
                    }
                    
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sql &&
                                   (sql.Number == 2601 || sql.Number == 2627))
                {
                    ModelState.AddModelError("Nombre", "Ya existe una categoría con ese nombre.");
                    return View(categoria);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        [Authorize(Roles = Configs.ADMIN_ROLE)]

        public IActionResult Delete(int? id) => NotFound();


        // POST: Categorias/Delete/5
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) => NotFound();

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}
