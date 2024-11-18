using Ecommerce.Frontend.Models;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;

namespace Ecommerce.Frontend.Services
{
    public interface IServicioProducto
    {
        Task<string> ObtenerCodigo();

        Task<IEnumerable<Producto>> ObtenerProductosAsync();

        Task<IEnumerable<Categoria>> ObtenerCategoriasAsync();

        Task<ProductoDTO?> BuscarProductoAsync(int id);

        Task<bool> ActualizarProductoAsync(Producto producto);

        Task<List<ProductosViewModel>> ObtenerResumenProductos();

        Task<bool> GuardarProductoAsync(Producto producto);
    }
}