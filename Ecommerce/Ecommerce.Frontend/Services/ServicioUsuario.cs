using Ecommerce.Shared.Entities;
using Newtonsoft.Json;

namespace Ecommerce.Frontend.Services
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly HttpClient _httpClient;

        public ServicioUsuario(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
        }

        public async Task<Usuario> GetUsuarioByEmail(string email)
        {
            var response = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var json = await response.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(json);
            return usuario!;
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuariosAsync()
        {
            var response = await _httpClient.GetAsync("/api/Usuarios");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(content);
                return usuarios!;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return [];
            }
            return [];
        }
    }
}