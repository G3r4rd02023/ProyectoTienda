using Ecommerce.Shared.Entities;
using Newtonsoft.Json;

namespace Ecommerce.Frontend.Services
{
    public class ServicioProducto : IServicioProducto
    {
        private readonly HttpClient _httpClient;

        public ServicioProducto(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
        }

        public async Task<string> ObtenerCodigo()
        {
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
    }
}