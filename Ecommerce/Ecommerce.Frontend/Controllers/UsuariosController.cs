using Ecommerce.Frontend.Services;
using Ecommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Ecommerce.Frontend.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;

        public UsuariosController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _usuario = usuario;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Usuarios");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(content);
                return View("Index", usuarios);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(new List<Usuario>());
        }
    }
}