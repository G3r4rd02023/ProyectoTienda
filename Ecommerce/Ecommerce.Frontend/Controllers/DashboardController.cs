using Ecommerce.Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Frontend.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;
        private readonly IServicioProducto _producto;
        private readonly IServicioVenta _venta;

        public DashboardController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario, IServicioProducto producto, IServicioVenta venta)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _usuario = usuario;
            _producto = producto;
            _venta = venta;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuario.ObtenerUsuariosAsync();
            ViewBag.CantidadUsuarios = usuarios.Count();
            var productos = await _producto.ObtenerProductosAsync();
            ViewBag.CantidadProductos = productos.Count();
            var ventas = await _venta.ObtenerVentasAsync();
            ViewBag.NuevosPedidos = ventas.Where(v => v.EstadoPedido == Shared.Enums.EstadoPedido.Nuevo).Count();
            ViewBag.CantidadVentasConfirmadas = ventas.Where(v => v.EstadoPedido == Shared.Enums.EstadoPedido.Confirmado).Count();
            var ventasTemporales = await _venta.ObtenerTemporalesAsync();
            return View(ventasTemporales.ToList());
        }

        public async Task<IActionResult> ResumenVenta()
        {
            var resumen = await _venta.ObtenerResumenVenta();
            return Ok(resumen);
        }

        public async Task<IActionResult> ResumenProducto()
        {
            var resumen = await _producto.ObtenerResumenProductos();
            return Ok(resumen);
        }
    }
}