using System.Threading.Tasks;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Carrito_B.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Carrito_B.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<Persona> _userManager;

        public PerfilController(UserManager<Persona> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var persona = await _userManager.GetUserAsync(User);

            if (persona == null)
            {
                return RedirectToAction("IniciarSesion", "Account");
            }

            var vm = new Perfil
            {
                Email = persona.Email,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                Telefono = persona.Telefono,
                Direccion = persona.Direccion,
                Dni = persona.DNI
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Perfil model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var persona = await _userManager.GetUserAsync(User);

            persona.Direccion = model.Direccion;
            persona.Telefono = model.Telefono;

            var result = await _userManager.UpdateAsync(persona);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["MensajePerfil"] = "Perfil actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
