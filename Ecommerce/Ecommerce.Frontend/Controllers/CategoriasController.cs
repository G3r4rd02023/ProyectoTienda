using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.Frontend.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoriasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Categorias");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<IEnumerable<Categoria>>(content);
                return View("Index", categorias);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(new List<Categoria>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(categoria);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Categorias/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "La categoria " + categoria.Nombre + "se ha creado exitosamente";
                    return RedirectToAction("Index", "Categorias");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "No se puedo agregar la categoria " + categoria.Nombre + " porque ya existe.";
                    return RedirectToAction("Index", "Categorias");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al crear una nueva categoria!!!";
                    return RedirectToAction("Index", "Categorias");
                }
            }
            return View(categoria);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Categorias/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener la categoria.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var categoria = JsonConvert.DeserializeObject<Categoria>(content);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(categoria);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Categorias/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "La categoria se ha actualizado exitosamente";
                    return RedirectToAction("Index", "Categorias");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "No se pudo actualizar el nombre de la categoria a : " + categoria.Nombre + " porque ya existe una categoria con ese nombre";
                    return RedirectToAction("Index", "Categorias");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al editar la categoria!!!";
                    return RedirectToAction("Index", "Categorias");
                }
            }
            return View(categoria);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Categorias/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Categoria eliminada exitosamente!!!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar la categoria.";
                return RedirectToAction("Index");
            }
        }
    }
}