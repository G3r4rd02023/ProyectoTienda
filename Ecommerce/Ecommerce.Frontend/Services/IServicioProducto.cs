using Ecommerce.Frontend.Models;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;

namespace Ecommerce.Frontend.Services
{
    public interface IServicioProducto
    {
        Task<string> ObtenerCodigo(string usuario);

        Task<IEnumerable<Producto>> ObtenerProductosAsync(string usuario);

        Task<IEnumerable<Categoria>> ObtenerCategoriasAsync(string usuario);

        Task<ProductoDTO?> BuscarProductoAsync(int id, string usuario);

        Task<bool> ActualizarProductoAsync(Producto producto, string usuario);

        Task<List<ProductosViewModel>> ObtenerResumenProductos(string usuario);

        Task<bool> GuardarProductoAsync(Producto producto, string usuario);
    }
}