using Carrito_B.Helpers;
using Carrito_B.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Carrito_B.Data
{
    public class CarritoContext : IdentityDbContext<Persona, IdentityRole<int>, int>
    {
        public CarritoContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Persona>().ToTable("Personas");

            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = Configs.ADMIN_ROLE,
                    NormalizedName = Configs.ADMIN_ROLE.ToUpper(),
                    ConcurrencyStamp = "b6f8e6d4-9b5f-4a4d-9ef2-000000000001"
                },
                new IdentityRole<int>
                {
                    Id = 2,
                    Name = Configs.CLIENTE_ROLE,
                    NormalizedName = Configs.CLIENTE_ROLE.ToUpper(),
                    ConcurrencyStamp = "b6f8e6d4-9b5f-4a4d-9ef2-000000000002"
                },
                new IdentityRole<int>
                {
                    Id = 3,
                    Name = Configs.EMPLEADO_ROLE,
                    NormalizedName = Configs.EMPLEADO_ROLE.ToUpper(),
                    ConcurrencyStamp = "b6f8e6d4-9b5f-4a4d-9ef2-000000000003"
                }
            );

            modelBuilder.Entity<CarritoItem>()
                .HasOne(ci => ci.Carrito)
                .WithMany(c => c.CarritoItems)
                .HasForeignKey(ci => ci.CarritoId);

            modelBuilder.Entity<Producto>()
                .HasIndex(p => new { p.MarcaId, p.Nombre })
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioVigente)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nombre)
                .IsUnique();

            modelBuilder.Entity<Marca>()
                .HasIndex(m => m.Nombre)
                .IsUnique();

            modelBuilder.Entity<Sucursal>()
                .HasIndex(s => s.Nombre)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.DNI)
                .IsUnique();

            modelBuilder.Entity<Empleado>()
                .HasIndex(e => e.Legajo)
                .IsUnique();

            modelBuilder.Entity<CarritoItem>()
                .HasOne(ci => ci.Producto)
                .WithMany(p => p.CarritoItems)
                .HasForeignKey(ci => ci.ProductoId);

            modelBuilder.Entity<StockItem>()
                .HasOne(si => si.Sucursal)
                .WithMany(s => s.StockItems)
                .HasForeignKey(si => si.SucursalId);

            modelBuilder.Entity<StockItem>()
                .HasOne(si => si.Producto)
                .WithMany(p => p.StockItems)
                .HasForeignKey(si => si.ProductoId);
        }

        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Persona> Personas { get; set; }

    }

}
