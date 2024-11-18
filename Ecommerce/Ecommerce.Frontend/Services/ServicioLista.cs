using Ecommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Ecommerce.Frontend.Services
{
    public class ServicioLista : IServicioLista
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;

        public ServicioLista(IHttpClientFactory httpClientFactory, IServicioUsuario usuario)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _usuario = usuario;
        }

        public async Task<IEnumerable<SelectListItem>> GetListaCategorias(string usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario);
            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/Categorias");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<IEnumerable<Categoria>>(content);
                var lista = categorias!.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }).ToList();

                lista.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Seleccione la categoria"
                });
                return lista;
            }

            return [];
        }
    }
}