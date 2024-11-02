using CloudinaryDotNet;
using Ecommerce.Frontend.Services;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Enums;
using CloudinaryDotNet.Actions;

namespace Ecommerce.Frontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;
        private readonly Cloudinary _cloudinary;

        public AccountController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario, Cloudinary cloudinary)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _usuario = usuario;
            _cloudinary = cloudinary;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Account/Login", content);
                var user = await _usuario.GetUsuarioByEmail(model.CorreoElectronico);

                if (response.IsSuccessStatusCode)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.CorreoElectronico),
                        new Claim(ClaimTypes.Role, user.Rol.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    TempData["SuccessMessage"] = "El usuario " + user.Nombre + " ha iniciado sesion en el sistema";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        TempData["ErrorMessage"] = "El usuario se ha creado , pero necesita aprobacion del administrador, para acceder al sistema";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Usuario o clave incorrectos!!!  ";
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Registro()
        {
            var usuario = new Usuario
            {
                Rol = Rol.Usuario,
                Estado = "Nuevo",
                SesionActiva = false,
            };
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario, IFormFile? file)
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
                        return View(usuario);
                    }

                    var urlImagen = uploadResult.SecureUrl.ToString();
                    usuario.URLFoto = urlImagen;
                }

                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Account/Registro", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Usuario " + usuario.Nombre + " registrado exitosamente!";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error, no se pudo crear el usuario";
                }
            }
            return View(usuario);
        }
    }
}