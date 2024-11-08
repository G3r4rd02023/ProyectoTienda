using Ecommerce.Frontend.Models;
using Ecommerce.Frontend.Services;
using Ecommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ecommerce.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioProducto _producto;
        private readonly IServicioUsuario _usuario;
        private readonly IServicioVenta _venta;

        public HomeController(IHttpClientFactory httpClientFactory, IServicioProducto producto, IServicioUsuario usuario, IServicioVenta venta)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _producto = producto;
            _usuario = usuario;
            _venta = venta;
        }

        public async Task<IActionResult> Index(string searchName, int? categoryId, int pageNumber = 1, int pageSize = 12)
        {
            var productos = await _producto.ObtenerProductosAsync();
            var categorias = await _producto.ObtenerCategoriasAsync();
            var usuario = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                productos = productos.Where(p => p.Nombre.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (categoryId.HasValue)
            {
                productos = productos.Where(p => p.CategoriaId == categoryId.Value).ToList();
            }

            int totalItems = productos.Count();
            var pagedProductos = productos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            HomeViewModel model = new()
            {
                Productos = pagedProductos,
                Categorias = categorias.ToList(),
                Cantidad = await _venta.ObtenerCantidad(User.Identity!.Name!),
                Usuario = usuario,
                SearchName = searchName,
                SelectedCategoryId = categoryId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var producto = await _producto.BuscarProductoAsync(id);
            return View(producto);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var producto = await _producto.BuscarProductoAsync(id);
            var usuario = await _usuario.GetUsuarioByEmail(User.Identity.Name!);

            VentaTemporal ventaTemporal = new()
            {
                Producto = producto,
                Usuario = usuario,
                Cantidad = 1
            };

            var response = await _venta.GuardarVentaTemporalAsync(ventaTemporal);
            if (response)
            {
                TempData["SuccessMessage"] = "El producto " + producto!.Nombre + " se ha agregado exitosamente al carrito";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Error al agregar el producto " + producto!.Nombre + " ";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ShowCart()
        {
            Usuario usuario = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            var ventasTemporales = await _venta.ObtenerTemporalesAsync();
            List<VentaTemporal> ventaTemporal = ventasTemporales.Where(t => t.Usuario!.Id == usuario.Id).ToList();

            CartViewModel model = new()
            {
                Usuario = usuario,
                VentasTemporales = ventaTemporal
            };

            return View(model);
        }

        public async Task<IActionResult> Disminuir(int id)
        {
            VentaTemporal ventaTemporal = await _venta.ObtenerVentaTemporalAsync(id);

            if (ventaTemporal.Cantidad > 1)
            {
                ventaTemporal.Cantidad--;
                await _venta.ActualizarVentaTemporalAsync(ventaTemporal);
            }
            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> Incrementar(int id)
        {
            VentaTemporal ventaTemporal = await _venta.ObtenerVentaTemporalAsync(id);

            ventaTemporal.Cantidad++;
            await _venta.ActualizarVentaTemporalAsync(ventaTemporal);
            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> Delete(int id)
        {
            VentaTemporal ventaTemporal = await _venta.ObtenerVentaTemporalAsync(id);
            await _venta.EliminarVentaTemporalAsync(ventaTemporal);
            return RedirectToAction(nameof(ShowCart));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}