using Ecommerce.Shared.Entities;

namespace Ecommerce.Frontend.Services
{
    public interface IServicioVenta
    {
        Task<bool> GuardarVentaTemporalAsync(VentaTemporal ventaTemporal);

        Task<IEnumerable<VentaTemporal>> ObtenerTemporalesAsync();

        Task<int> ObtenerCantidad(string usuario);
    }
}