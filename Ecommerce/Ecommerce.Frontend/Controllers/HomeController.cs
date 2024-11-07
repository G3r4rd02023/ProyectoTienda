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

        public HomeController(IHttpClientFactory httpClientFactory, IServicioProducto producto, IServicioUsuario usuario)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _producto = producto;
            _usuario = usuario;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _producto.ObtenerProductosAsync();
            var categorias = await _producto.ObtenerCategoriasAsync();

            // Crear el modelo y paginar productos
            HomeViewModel model = new()
            {
                Productos = productos.ToList(),
                Categorias = categorias.ToList()
            };

            return View(model);
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