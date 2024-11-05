using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.Frontend.Models;
using Ecommerce.Frontend.Services;
using Ecommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.Frontend.Controllers
{
    public class ProductosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly Cloudinary _cloudinary;
        private readonly IServicioProducto _producto;
        private readonly IServicioLista _lista;

        public ProductosController(IHttpClientFactory httpClientFactory, Cloudinary cloudinary, IServicioProducto producto, IServicioLista lista)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _cloudinary = cloudinary;
            _producto = producto;
            _lista = lista;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Productos");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var productos = JsonConvert.DeserializeObject<IEnumerable<Producto>>(content);
                return View("Index", productos);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(new List<Producto>());
        }

        public async Task<IActionResult> Create()
        {
            ProductoDTO producto = new()
            {
                Codigo = await _producto.ObtenerCodigo(),
                Categorias = await _lista.GetListaCategorias()
            };
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoDTO producto, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        AssetFolder = "tecnologers"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error al cargar la imagen.");
                        return View(producto);
                    }

                    var urlImagen = uploadResult.SecureUrl.ToString();
                    producto.URLFoto = urlImagen;
                }

                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Productos/", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "El producto " + producto.Nombre + " se ha creado exitosamente";
                    return RedirectToAction("Index", "Productos");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "No se puedo agregar el producto " + producto.Nombre + " porque ya existe.";
                    return RedirectToAction("Index", "Productos");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al crear un nuevo producto!!!";
                    return RedirectToAction("Index", "Productos");
                }
            }
            producto.Categorias = await _lista.GetListaCategorias();
            return View(producto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Productos/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener el producto.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var producto = JsonConvert.DeserializeObject<ProductoDTO>(content);
            if (producto == null)
            {
                return NotFound();
            }

            producto.Categorias = await _lista.GetListaCategorias();
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductoDTO producto, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        AssetFolder = "tecnologers"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error al cargar la imagen.");
                        return View(producto);
                    }

                    var urlImagen = uploadResult.SecureUrl.ToString();
                    producto.URLFoto = urlImagen;
                }
                else
                {
                    var productoResponse = await _httpClient.GetAsync($"/api/Productos/{producto.Id}");
                    var productoContent = await productoResponse.Content.ReadAsStringAsync();
                    var productoExistente = JsonConvert.DeserializeObject<ProductoDTO>(productoContent);
                    producto.URLFoto = productoExistente!.URLFoto;
                }
                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Productos/{producto.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "El producto se ha actualizado exitosamente";
                    return RedirectToAction("Index", "Productos");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al editar el producto!!!";
                    return RedirectToAction("Index", "Productos");
                }
            }
            return View(producto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Productos/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Producto eliminado exitosamente!!!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el producto.";
                return RedirectToAction("Index");
            }
        }
    }
}