using Ecommerce.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaTemporal> VentasTemporal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Categoria>().HasIndex(c => c.Nombre).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(c => c.DNI).IsUnique();
            modelBuilder.Entity<Producto>().HasIndex(c => c.Codigo).IsUnique();
        }
    }
}