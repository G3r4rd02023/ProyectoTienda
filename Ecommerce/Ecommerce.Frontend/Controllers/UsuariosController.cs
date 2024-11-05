using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.Frontend.Models;
using Ecommerce.Frontend.Services;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Ecommerce.Frontend.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;
        private readonly Cloudinary _cloudinary;

        public UsuariosController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario, Cloudinary cloudinary)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _usuario = usuario;
            _cloudinary = cloudinary;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Usuarios");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(content);
                return View("Index", usuarios);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(new List<Usuario>());
        }

        public IActionResult Create()
        {
            var usuario = new Usuario()
            {
                Estado = "Activo",
                SesionActiva = false,
            };
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario, IFormFile? file)
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
                var response = await _httpClient.PostAsync("/api/Usuarios/", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "El nuevo administrador se ha creado exitosamente";
                    return RedirectToAction("Index", "Usuarios");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al crear el usuario administrador!!!";
                }
            }
            return View(usuario);
        }

        public async Task<IActionResult> Access(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Usuarios/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener el usuario.";
                return RedirectToAction("Index");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Usuario>(content);
                if (user == null)
                {
                    return NotFound();
                }

                var model = new AllowUserViewModel()
                {
                    Nombre = user.Nombre,
                    Correo = user.Correo,
                    Estado = user.Estado,
                    Rol = user.Rol
                };

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Access(int id, AllowUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{model.Correo}");

                if (userResponse.IsSuccessStatusCode)
                {
                    var userContent = await userResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<Usuario>(userContent);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    var usuario = new Usuario
                    {
                        Id = user.Id,
                        SesionActiva = false,
                        Contrasena = user.Contrasena,
                        Correo = user.Correo,
                        Direccion = user.Direccion,
                        DNI = user.DNI,
                        Estado = model.Estado,
                        Nombre = user.Nombre,
                        Rol = model.Rol,
                        URLFoto = user.URLFoto,
                        Telefono = user.Telefono
                    };

                    var json = JsonConvert.SerializeObject(usuario);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"/api/Usuarios/{usuario.Id}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "El usuario ha sido modificado Exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ocurrio un error al actualizar el usuario";
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Direccion = user.Direccion,
                DNI = user.DNI,
                Telefono = user.Telefono,
                URLFoto = user.URLFoto,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
                if (user == null)
                {
                    return NotFound();
                }

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
                        return View(user);
                    }

                    var urlImagen = uploadResult.SecureUrl.ToString();
                    model.URLFoto = urlImagen;
                }
                else
                {
                    model.URLFoto = user.URLFoto;
                }

                var usuarioExistente = new Usuario
                {
                    SesionActiva = user.SesionActiva,
                    Contrasena = user.Contrasena,
                    Correo = user.Correo,
                    Direccion = model.Direccion,
                    DNI = model.DNI,
                    Estado = user.Estado,
                    Nombre = model.Nombre,
                    Rol = user.Rol,
                    URLFoto = model.URLFoto,
                    Telefono = model.Telefono,
                    Id = user.Id
                };

                var json = JsonConvert.SerializeObject(usuarioExistente);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/Usuarios/{user.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "La información se actualizó exitosamente!!!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al actualizar usuario!!";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Usuarios/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente!!!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el usuario.";
                return RedirectToAction("Index");
            }
        }
    }
}