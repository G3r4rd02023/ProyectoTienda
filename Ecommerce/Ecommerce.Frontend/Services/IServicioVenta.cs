using Ecommerce.Frontend.Models;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Frontend.Services
{
    public interface IServicioVenta
    {
        Task<bool> GuardarVentaTemporalAsync(VentaTemporal ventaTemporal);

        Task<IEnumerable<VentaTemporal>> ObtenerTemporalesAsync();

        Task<IEnumerable<Venta>> ObtenerVentasAsync();

        Task<int> ObtenerCantidad(string usuario);

        Task<Venta> ObtenerVentaAsync(int id);

        Task<VentaTemporal> ObtenerVentaTemporalAsync(int id);

        Task<bool> ActualizarVentaTemporalAsync(VentaTemporal ventaTemporal);

        Task<bool> ActualizarVentaAsync(Venta venta);

        Task<bool> EliminarVentaTemporalAsync(VentaTemporal ventaTemporal);

        Task<Response> ProcesarVenta(CartViewModel model);

        Task<Response> CancelarVenta(int id);

        Task<List<VentasViewModel>> ObtenerResumenVenta();
    }
}