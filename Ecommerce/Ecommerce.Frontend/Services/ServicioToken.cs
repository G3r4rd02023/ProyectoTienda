using Ecommerce.Frontend.Models;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.Frontend.Services
{
    public class ServicioToken
    {
        public async Task<string> Autenticar(Usuario userToken)
        {
            var cliente = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7102/")
            };

            var credenciales = new UsuarioToken()
            {
                Correo = userToken.Correo,
                Contrasena = userToken.Contrasena
            };

            var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync("api/Authentication/ValidarUsuario", content);
            var json = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<TokenViewModel>(json);
            var token = resultado!.Token;
            return token;
        }
    }
}