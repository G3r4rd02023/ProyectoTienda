using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Frontend.Services
{
    public class ServicioLista : IServicioLista
    {
        private readonly HttpClient _httpClient;

        public ServicioLista(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
        }

        public Task<IEnumerable<SelectListItem>> GetListaRoles()
        {
            throw new NotImplementedException();
        }
    }
}