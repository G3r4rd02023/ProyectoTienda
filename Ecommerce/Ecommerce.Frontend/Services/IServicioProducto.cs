using Ecommerce.Frontend.Models;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Frontend.Services
{
    public interface IServicioProducto
    {
        Task<string> ObtenerCodigo();

        Task<IEnumerable<Producto>> ObtenerProductosAsync();

        Task<IEnumerable<Categoria>> ObtenerCategoriasAsync();

        Task<IQueryable<Producto>> QueryProductosAsync();
    }
}