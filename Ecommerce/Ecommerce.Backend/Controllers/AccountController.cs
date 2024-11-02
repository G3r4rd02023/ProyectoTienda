using Ecommerce.Backend.Data;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.Correo == login.CorreoElectronico);

            if (usuario != null && usuario.Estado == "Activo")
            {
                if (BCrypt.Net.BCrypt.Verify(login.Contrasena, usuario.Contrasena))
                {
                    return Ok(new { Message = "Inicio de sesión exitoso.", isSuccess = true, isNewUser = usuario.Estado });
                }
            }
            else if (usuario!.Estado == "Nuevo")
            {
                return Conflict(new { Message = "Inicio de sesión fallido. El usuario es nuevo pero aún no está activo.", isSuccess = false });
            }

            return Unauthorized(new { Message = "Inicio de sesión fallido. Usuario o contraseña incorrectos.", isSuccess = false, token = "" });
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Usuario registrado exitosamente." });
        }
    }
}