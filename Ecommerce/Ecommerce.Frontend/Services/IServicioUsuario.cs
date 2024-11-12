using Ecommerce.Shared.Entities;

namespace Ecommerce.Frontend.Services
{
    public interface IServicioUsuario
    {
        Task<Usuario> GetUsuarioByEmail(string email);

        Task<IEnumerable<Usuario>> ObtenerUsuariosAsync();
    }
}