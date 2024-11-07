using Ecommerce.Frontend.Models;
using Ecommerce.Frontend.Services;
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

        public async Task<IActionResult> Index(string searchName, int? categoryId, int pageNumber = 1, int pageSize = 12)
        {
            var productos = await _producto.ObtenerProductosAsync();
            var categorias = await _producto.ObtenerCategoriasAsync();

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
                SearchName = searchName,
                SelectedCategoryId = categoryId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
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