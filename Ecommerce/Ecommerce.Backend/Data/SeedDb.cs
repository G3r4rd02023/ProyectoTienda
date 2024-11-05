using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await ValidarUsuariosAsync("Tecnologers", "tecnologershn@gmail.com", "123456", Rol.Administrador);
            await CrearCategoriasAsync();
        }

        private async Task CrearCategoriasAsync()
        {
            if (!_context.Categorias.Any())
            {
                _context.Categorias.Add(new Categoria { Nombre = "Tecnología" });
                _context.Categorias.Add(new Categoria { Nombre = "Ropa" });
                _context.Categorias.Add(new Categoria { Nombre = "Gamer" });
                _context.Categorias.Add(new Categoria { Nombre = "Nutricion" });
                _context.Categorias.Add(new Categoria { Nombre = "Belleza" });
                _context.Categorias.Add(new Categoria { Nombre = "Deportes" });
                _context.Categorias.Add(new Categoria { Nombre = "Hogar" });
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Usuario> ValidarUsuariosAsync(string nombre, string correo, string pass, Rol rol)
        {
            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuarioExistente != null)
            {
                return usuarioExistente;
            }

            Usuario usuario = new()
            {
                DNI = "0801-2000-12345",
                Direccion = "Calle 123 45 67",
                Estado = "Activo",
                SesionActiva = true,
                Telefono = "3170-2187",
                Contrasena = pass,
                Correo = correo,
                Nombre = nombre,
                Rol = rol,
                URLFoto = "https://res.cloudinary.com/dbsaxzz05/image/upload/v1725662135/dqsw7mavp77po9xwjjgw.jpg"
            };

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}