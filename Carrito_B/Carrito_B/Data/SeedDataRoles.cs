using Carrito_B.Helpers;
using Carrito_B.Models;
using Carrito_B.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Carrito_B.Data
{
    public static class SeedDataRoles
    {
        public static async Task SeedRolesAndAdmin(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = services.GetRequiredService<UserManager<Persona>>();
            var db = services.GetRequiredService<CarritoContext>();

            if (!await roleManager.RoleExistsAsync(Configs.ADMIN_ROLE))
                await roleManager.CreateAsync(new IdentityRole<int>(Configs.ADMIN_ROLE));

            if (!await roleManager.RoleExistsAsync(Configs.CLIENTE_ROLE))
                await roleManager.CreateAsync(new IdentityRole<int>(Configs.CLIENTE_ROLE));

            if (!await roleManager.RoleExistsAsync(Configs.EMPLEADO_ROLE))
                await roleManager.CreateAsync(new IdentityRole<int>(Configs.EMPLEADO_ROLE));

            var adminEmail = "admin@ort.edu.ar";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var nuevoAdmin = new Persona
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Nombre = "Administrador",
                    Apellido = "Principal",
                    DNI = "00000000",
                    Telefono = "0000000000",
                    Direccion = "Oficina Central",
                    FechaAlta = DateTime.Now,
                };


                var createResult = await userManager.CreateAsync(nuevoAdmin, "Password1!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(nuevoAdmin, Configs.ADMIN_ROLE);
                }
            }

            var clienteEmail = "cliente1@ort.edu.ar";
            var clienteUser = await userManager.FindByEmailAsync(clienteEmail);

            if (clienteUser == null)
            {
                var nuevoCliente = new Cliente
                {
                    UserName = clienteEmail,
                    Email = clienteEmail,
                    EmailConfirmed = true,
                    Nombre = "ClienteUno",
                    Apellido = "EjemploUno",
                    DNI = "11111111",
                    Telefono = "1111111111",
                    Direccion = "Calle Falsa 123",
                    FechaAlta = DateTime.Now,
                    IdentificacionUnica = "C1111111111"
                };

                var createClienteResult = await userManager.CreateAsync(nuevoCliente, "Password1!");
                if (createClienteResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(nuevoCliente, Configs.CLIENTE_ROLE);
                }
            }

            var clienteEmail2 = "cliente2@ort.edu.ar";
            var clienteUser2 = await userManager.FindByEmailAsync(clienteEmail2);

            if (clienteUser2 == null)
            {
                var cliente2 = new Cliente
                {
                    UserName = clienteEmail2,
                    Email = clienteEmail2,
                    EmailConfirmed = true,
                    Nombre = "ClienteDos",
                    Apellido = "EjemploDos",
                    DNI = "22222222",
                    Telefono = "2222222222",
                    Direccion = "Calle Falsa 456",
                    FechaAlta = DateTime.Now,
                    IdentificacionUnica = "C2222222222"
                };

                var createClienteResult2 = await userManager.CreateAsync(cliente2, "Password1!");
                if (createClienteResult2.Succeeded)
                {
                    await userManager.AddToRoleAsync(cliente2, Configs.CLIENTE_ROLE);
                }
            }

            var empleadoEmail = "empleado1@ort.edu.ar";
            var empleadoUser = await userManager.FindByEmailAsync(empleadoEmail);

            if (empleadoUser == null)
            {
                var nuevoEmpleado = new Empleado
                {
                    UserName = empleadoEmail,
                    Email = empleadoEmail,
                    EmailConfirmed = true,
                    Nombre = "EmpleadoUno",
                    Apellido = "EjemploTres",
                    DNI = "33333333",
                    Telefono = "3333333333",
                    Direccion = "Siempre Viva 742",
                    FechaAlta = DateTime.Now,
                    Legajo = 1
                };
                var createEmpleadoResult = await userManager.CreateAsync(nuevoEmpleado, "Password1!");
                if (createEmpleadoResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(nuevoEmpleado, Configs.EMPLEADO_ROLE);
                }
            }

            var empleadoEmail2 = "empleado2@ort.edu.ar";
            var empleadoUser2 = await userManager.FindByEmailAsync(empleadoEmail2);

            if (empleadoUser2 == null)
            {
                var empleado2 = new Empleado
                {
                    UserName = empleadoEmail2,
                    Email = empleadoEmail2,
                    EmailConfirmed = true,
                    Nombre = "EmpleadoDos",
                    Apellido = "EjemploCuatro",
                    DNI = "44444444",
                    Telefono = "4444444444",
                    Direccion = "Avenida Viva 532",
                    FechaAlta = DateTime.Now,
                    Legajo = 2
                };

                var createEmpleadoResult2 = await userManager.CreateAsync(empleado2, "Password1!");
                if (createEmpleadoResult2.Succeeded)
                {
                    await userManager.AddToRoleAsync(empleado2, Configs.EMPLEADO_ROLE);
                }
            }

            clienteUser = await userManager.FindByEmailAsync(clienteEmail);
            clienteUser2 = await userManager.FindByEmailAsync(clienteEmail2);

            // ---------- Compra ejemplo clienteUser (1) ----------
            if (clienteUser != null)
            {
                var fecha1 = new DateTime(2025, 11, 20, 10, 0, 0);
                var exists1 = await db.Compras.AnyAsync(c => c.Carrito.ClienteId == clienteUser.Id && c.Fecha == fecha1);
                if (!exists1)
                {
                    var carrito = new Carrito { Activo = false, ClienteId = clienteUser.Id };
                    db.Carritos.Add(carrito);
                    await db.SaveChangesAsync();

                    var p1 = await db.Productos.FindAsync(1);
                    var p4 = await db.Productos.FindAsync(4);

                    var ci1 = new CarritoItem { CarritoId = carrito.Id, ProductoId = 1, Cantidad = 2, ValorUnitario = p1?.PrecioVigente ?? 0m };
                    var ci2 = new CarritoItem { CarritoId = carrito.Id, ProductoId = 4, Cantidad = 1, ValorUnitario = p4?.PrecioVigente ?? 0m };
                    db.CarritoItems.AddRange(ci1, ci2);

                    var s1 = await db.StockItems.FirstOrDefaultAsync(si => si.ProductoId == 1 && si.SucursalId == 1);
                    if (s1 != null) s1.Cantidad = Math.Max(0, s1.Cantidad - 2);
                    var s4 = await db.StockItems.FirstOrDefaultAsync(si => si.ProductoId == 4 && si.SucursalId == 1);
                    if (s4 != null) s4.Cantidad = Math.Max(0, s4.Cantidad - 1);

                    await db.SaveChangesAsync();

                    var total1 = ci1.Cantidad * ci1.ValorUnitario + ci2.Cantidad * ci2.ValorUnitario;
                    var compra1 = new Compra
                    {
                        CarritoId = carrito.Id,
                        Cliente = (Cliente)clienteUser,
                        Fecha = fecha1,
                        SucursalId = 1,
                        Total = total1
                    };

                    // Asegurar que el carrito usado quede inactivo y crear uno nuevo activo para el cliente
                    carrito.Activo = false;
                    db.Carritos.Update(carrito);
                   

                    db.Compras.Add(compra1);
                    await db.SaveChangesAsync();
                }
            }

            // ---------- Compra ejemplo clienteUser2 (2) ----------
            if (clienteUser2 != null)
            {
                var fecha2 = new DateTime(2025, 11, 20, 11, 30, 0);
                var exists2 = await db.Compras.AnyAsync(c => c.Carrito.ClienteId == clienteUser2.Id && c.Fecha == fecha2);
                if (!exists2)
                {
                    var carrito2 = new Carrito { Activo = false, ClienteId = clienteUser2.Id };
                    db.Carritos.Add(carrito2);
                    await db.SaveChangesAsync();

                    var p2 = await db.Productos.FindAsync(2);
                    var p6 = await db.Productos.FindAsync(6);

                    var ci21 = new CarritoItem { CarritoId = carrito2.Id, ProductoId = 2, Cantidad = 1, ValorUnitario = p2?.PrecioVigente ?? 0m };
                    var ci22 = new CarritoItem { CarritoId = carrito2.Id, ProductoId = 6, Cantidad = 2, ValorUnitario = p6?.PrecioVigente ?? 0m };
                    db.CarritoItems.AddRange(ci21, ci22);

                    var s2 = await db.StockItems.FirstOrDefaultAsync(si => si.ProductoId == 2 && si.SucursalId == 2);
                    if (s2 != null) s2.Cantidad = Math.Max(0, s2.Cantidad - 1);
                    var s6 = await db.StockItems.FirstOrDefaultAsync(si => si.ProductoId == 6 && si.SucursalId == 2);
                    if (s6 != null) s6.Cantidad = Math.Max(0, s6.Cantidad - 2);

                    await db.SaveChangesAsync();

                    var total2 = ci21.Cantidad * ci21.ValorUnitario + ci22.Cantidad * ci22.ValorUnitario;
                    var compra2 = new Compra
                    {
                        CarritoId = carrito2.Id,
                        Cliente = (Cliente)clienteUser2,
                        Fecha = fecha2,
                        SucursalId = 2,
                        Total = total2
                    };

                    carrito2.Activo = false;
                    db.Carritos.Update(carrito2);
                    db.Carritos.Add(new Carrito { Activo = true, ClienteId = clienteUser2.Id });

                    db.Compras.Add(compra2);
                    await db.SaveChangesAsync();
                }
            }

            // ---------- Compra ejemplo clienteUser (3) ----------
            if (clienteUser != null)
            {
                var fecha3 = new DateTime(2025, 11, 20, 12, 45, 0);
                var exists3 = await db.Compras.AnyAsync(c => c.Carrito.ClienteId == clienteUser.Id && c.Fecha == fecha3);
                if (!exists3)
                {
                    var carrito3 = new Carrito { Activo = false, ClienteId = clienteUser.Id };
                    db.Carritos.Add(carrito3);
                    await db.SaveChangesAsync();

                    var p10 = await db.Productos.FindAsync(10);
                    var p1b = await db.Productos.FindAsync(1);

                    var ci31 = new CarritoItem { CarritoId = carrito3.Id, ProductoId = 10, Cantidad = 3, ValorUnitario = p10?.PrecioVigente ?? 0m };
                    var ci32 = new CarritoItem { CarritoId = carrito3.Id, ProductoId = 1, Cantidad = 1, ValorUnitario = p1b?.PrecioVigente ?? 0m };
                    db.CarritoItems.AddRange(ci31, ci32);

                    var s10 = await db.StockItems.FirstOrDefaultAsync(si => si.ProductoId == 10 && si.SucursalId == 5);
                    if (s10 != null) s10.Cantidad = Math.Max(0, s10.Cantidad - 3);
                    var s1b = await db.StockItems.FirstOrDefaultAsync(si => si.ProductoId == 1 && si.SucursalId == 5);
                    if (s1b != null) s1b.Cantidad = Math.Max(0, s1b.Cantidad - 1);

                    await db.SaveChangesAsync();

                    var total3 = ci31.Cantidad * ci31.ValorUnitario + ci32.Cantidad * ci32.ValorUnitario;
                    var compra3 = new Compra
                    {
                        CarritoId = carrito3.Id,
                        Cliente = (Cliente)clienteUser,
                        Fecha = fecha3,
                        SucursalId = 5,
                        Total = total3
                    };

                    carrito3.Activo = false;
                    db.Carritos.Update(carrito3);
                   

                    db.Compras.Add(compra3);
                    await db.SaveChangesAsync();
                }
            }

            // Finalmente: asegurar que si por alguna razón no existe carrito activo se cree uno
            var changed = false;
            if (clienteUser != null)
            {
                var activo = await db.Carritos.FirstOrDefaultAsync(c => c.ClienteId == clienteUser.Id && c.Activo);
                if (activo == null)
                {
                    db.Carritos.Add(new Carrito { Activo = true, ClienteId = clienteUser.Id });
                    changed = true;
                }
            }

            if (clienteUser2 != null)
            {
                var activo2 = await db.Carritos.FirstOrDefaultAsync(c => c.ClienteId == clienteUser2.Id && c.Activo);
                if (activo2 == null)
                {
                    db.Carritos.Add(new Carrito { Activo = true, ClienteId = clienteUser2.Id });
                    changed = true;
                }
            }

            if (changed)
                await db.SaveChangesAsync();
        }
    }
}

