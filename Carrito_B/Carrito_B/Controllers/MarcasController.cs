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
    [Authorize(Roles = $"{Configs.ADMIN_ROLE},{Configs.EMPLEADO_ROLE}")]

    public class MarcasController : Controller
    {
        private readonly CarritoContext _context;

        public MarcasController(CarritoContext context)
        {
            _context = context;
        }

        // GET: Marcas
        public IActionResult Index()
        {
            return View(_context.Marcas.ToList());
        }

        // GET: Marcas/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Marcas == null)
            {
                return NotFound();
            }

            var marca = _context.Marcas
                .FirstOrDefault(m => m.Id == id);
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }

        
        // GET: Marcas/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: Marcas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nombre,Descripcion")] Marca marca)
        {
            marca.Nombre = marca.Nombre?.Trim();
            marca.Descripcion = marca.Descripcion?.Trim();

            bool existe = _context.Marcas
                .Any(m => m.Nombre.ToUpper() == marca.Nombre.ToUpper());

            if (existe)
            {
                ModelState.AddModelError("Nombre", "Ya existe una marca con ese nombre.");
            }

            if (!ModelState.IsValid)
            {
                return View(marca);
            }

            try
            {
                _context.Marcas.Add(marca);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql &&
                                               (sql.Number == 2601 || sql.Number == 2627))
            {
                ModelState.AddModelError("Nombre", "Ya existe una marca con ese nombre.");
                return View(marca);
            }
        }

        // POST: Marcas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(int id, [Bind("Id,Nombre,Descripcion")] Marca marca)
        {
            if (id != marca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var MarcaEnDB = _context.Marcas.Find(marca.Id);
                    if (MarcaEnDB != null)
                    {
                        MarcaEnDB.Nombre = marca.Nombre;
                        MarcaEnDB.Descripcion = marca.Descripcion;
                        _context.Marcas.Update(MarcaEnDB);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcaExists(marca.Id))
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
            return View(marca);
        }

        // GET: Marcas/Delete/5
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        public IActionResult Delete(int? id) => NotFound();


        // POST: Marcas/Delete/5
        [Authorize(Roles = Configs.ADMIN_ROLE)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) => NotFound();

        private bool MarcaExists(int id)
        {
            return _context.Marcas.Any(e => e.Id == id);
        }
    }
}
