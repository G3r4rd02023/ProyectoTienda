using Ecommerce.Frontend.Models;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Ecommerce.Frontend.Services
{
    public class ServicioProducto : IServicioProducto
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;

        public ServicioProducto(IHttpClientFactory httpClientFactory, IServicioUsuario usuario)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _usuario = usuario;
        }

        public async Task<string> ObtenerCodigo(string usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario);
            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/Productos");
            var content = await response.Content.ReadAsStringAsync();
            var productos = JsonConvert.DeserializeObject<IEnumerable<Producto>>(content);
            var lastCodigo = productos!.OrderByDescending(x => x.Id).Select(p => p.Codigo).FirstOrDefault();
            int lastNumber = 0;
            if (!string.IsNullOrEmpty(lastCodigo) && lastCodigo.Length > 2)
            {
                int.TryParse(lastCodigo.Substring(2), out lastNumber);
            }
            var codigo = $"P-{(lastNumber + 1).ToString("D5")}";
            return codigo;
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosAsync(string usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario);
            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/Productos");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Producto>>(content)!;
            }
            return [];
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasAsync(string usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario);
            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/Categorias");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Categoria>>(content)!;
            }
            return [];
        }

        public async Task<ProductoDTO?> BuscarProductoAsync(int id, string usuario)
        {
            try
            {
                var user = await _usuario.GetUsuarioByEmail(usuario);
                var servicioToken = new ServicioToken();
                var token = await servicioToken.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"/api/Productos/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var producto = JsonConvert.DeserializeObject<ProductoDTO>(content);
                    return producto;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> ActualizarProductoAsync(Producto producto, string usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario);
            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonConvert.SerializeObject(producto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Productos/{producto.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<List<ProductosViewModel>> ObtenerResumenProductos(string usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario);
            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/Productos/Resumen");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resumen = JsonConvert.DeserializeObject<List<ProductosViewModel>>(content);
                return resumen!;
            }
            return [];
        }

        public async Task<bool> GuardarProductoAsync(Producto producto, string usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario);
            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonConvert.SerializeObject(producto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Productos/", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}