using Carrito_B.Data;
using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Globalization;


namespace Carrito_B
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            builder.Services.AddDbContext<CarritoContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<Persona, IdentityRole<int>>()
                .AddEntityFrameworkStores<CarritoContext>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
            });

            builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                opciones =>
                {
                    opciones.LoginPath = "/Account/IniciarSesion";
                    opciones.AccessDeniedPath = "/Account/AccesoDenegado";
                    opciones.Cookie.Name = "IdentidadCarritoApp";
                });

            #region Cultura 
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources"); // <- Para soportar diferentes indiomas, ejemplo ingles y espa�ol.

            const string culturaPredeterminada = "es-AR";
            var culturasSoportadas = new[]
            {
                new CultureInfo(culturaPredeterminada),
                new CultureInfo("en-US")
            };

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culturaPredeterminada);
                options.SupportedCultures = culturasSoportadas;
                options.SupportedUICultures = culturasSoportadas;
            });

            //Adicionalmente, hay que resulver del lado del cliente con Jquery asique agregaremos lineas en:  wwwroot/js/site.js
            #endregion

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<CarritoContext>();
                db.Database.Migrate();

                await SeedDataRoles.SeedRolesAndAdmin(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
     
    }
}
