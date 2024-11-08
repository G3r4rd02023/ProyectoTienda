using Ecommerce.Shared.Entities;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.Frontend.Services
{
    public class ServicioVenta : IServicioVenta
    {
        private readonly HttpClient _httpClient;

        public ServicioVenta(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
        }

        public async Task<bool> ActualizarVentaTemporalAsync(VentaTemporal ventaTemporal)
        {
            var json = JsonConvert.SerializeObject(ventaTemporal);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Temporales/{ventaTemporal.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> EliminarVentaTemporalAsync(VentaTemporal ventaTemporal)
        {
            var response = await _httpClient.DeleteAsync($"/api/Temporales/{ventaTemporal.Id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> GuardarVentaTemporalAsync(VentaTemporal ventaTemporal)
        {
            var json = JsonConvert.SerializeObject(ventaTemporal);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Temporales/", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<int> ObtenerCantidad(string usuario)
        {
            var response = await _httpClient.GetAsync("/api/Temporales");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ventasTemporales = JsonConvert.DeserializeObject<IEnumerable<VentaTemporal>>(content)!;
                var carrito = ventasTemporales.Where(v => v.Usuario!.Correo == usuario).ToList();
                return carrito.Count;
            }
            return 0;
        }

        public async Task<IEnumerable<VentaTemporal>> ObtenerTemporalesAsync()
        {
            var response = await _httpClient.GetAsync("/api/Temporales");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<VentaTemporal>>(content)!;
            }
            return [];
        }

        public async Task<VentaTemporal> ObtenerVentaTemporalAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Temporales/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<VentaTemporal>(content)!;
            }
            return null!;
        }
    }
}