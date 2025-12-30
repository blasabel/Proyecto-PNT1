using Carrito_B.Helpers;
using Carrito_B.Models;
using Carrito_B.ViewModels;
using Carrito_B.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Carrito_B.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Persona> _userManager;
        private readonly SignInManager<Persona> _signInManager;
        private readonly CarritoContext _context;

        public AccountController(UserManager<Persona> userManager, SignInManager<Persona> signInManager, CarritoContext context)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._context = context;
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([Bind("Email,UserName,Password,ConfirmacionPassword,Nombre,Apellido,DNI,IdentificacionUnica,Telefono,Direccion")] RegistroUsuario registroUsuario)
        {
            if (ModelState.IsValid)
            {
                if (_context.Personas.Any(p => p.DNI == registroUsuario.DNI))
                {
                    ModelState.AddModelError("DNI", "El DNI ya está registrado.");
                    return View(registroUsuario);
                }
                else if (_context.Clientes.Any(c => c.IdentificacionUnica == registroUsuario.IdentificacionUnica))
                {
                    ModelState.AddModelError("IdentificacionUnica", "El CUIL ya está registrado.");
                    return View(registroUsuario);
                }
                else if (_context.Clientes.Any(p => p.Email == registroUsuario.Email))
                { 
                    ModelState.AddModelError("Email", "El email ya está registrado.");
                    return View(registroUsuario);
                }

                    var cliente = new Cliente
                    {
                        Email = registroUsuario.Email,
                        UserName = registroUsuario.UserName,
                        NormalizedEmail = registroUsuario.Email.ToUpperInvariant(),
                        NormalizedUserName = registroUsuario.UserName.ToUpperInvariant(),
                        Nombre = registroUsuario.Nombre,
                        Apellido = registroUsuario.Apellido,
                        DNI = registroUsuario.DNI,
                        IdentificacionUnica = registroUsuario.IdentificacionUnica,
                        Telefono = registroUsuario.Telefono,
                        Direccion = registroUsuario.Direccion,
                        FechaAlta = DateTime.Now
                    };


                var created = await _userManager.CreateAsync(cliente, registroUsuario.Password);

                if (created.Succeeded)
                {
                    await _userManager.AddToRoleAsync(cliente, Configs.CLIENTE_ROLE);

                    await _signInManager.SignInAsync(cliente, isPersistent: false);

             
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
                        _context.SaveChanges();
                    }

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in created.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(registroUsuario);
        }

        public IActionResult IniciarSesion(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> IniciarSesion(Login model, string returnUrl = null)
		{

            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
			{
				var resultado = await _signInManager.PasswordSignInAsync(
					model.UserName,
					model.Password,
					model.Recordame,
					lockoutOnFailure: false);

				if (resultado.Succeeded)
				{
					if (TempData.ContainsKey("PendingProductoId"))
					{
						return RedirectToAction("CompletarAddCarritoPostLogin", "Carritos");
					}

                    TempData.Remove("PendingProductoId");
                    TempData.Remove("PendingCantidad");

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
				}

				ModelState.AddModelError(string.Empty, "Inicio de sesión inválido");
			}

			return View(model);
		}

		public async Task<IActionResult> CerrarSesion()
        {
            TempData.Remove("PendingProductoId");
            TempData.Remove("PendingCantidad");

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
