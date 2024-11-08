using Ecommerce.Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Frontend.Controllers
{
    public class VentasController : Controller
    {
        private readonly IServicioVenta _venta;

        public VentasController(IServicioVenta venta)
        {
            _venta = venta;
        }

        public async Task<IActionResult> Index()
        {
            var venta = await _venta.ObtenerVentasAsync();
            return View(venta);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var venta = await _venta.ObtenerVentasAsync();
            var detalle = venta.FirstOrDefault(v => v.Id == id);
            return View(detalle);
        }
    }
}