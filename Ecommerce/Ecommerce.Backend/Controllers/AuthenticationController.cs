using Ecommerce.Backend.Data;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly string secretKey;
        private readonly DataContext _context;

        public AuthenticationController(IConfiguration config, DataContext context)
        {
            secretKey = config.GetSection("Jwt").GetValue<string>("key")!;
            _context = context;
        }

        [HttpPost]
        [Route("ValidarUsuario")]
        public IActionResult ValidarUsuario([FromBody] UsuarioToken userToken)
        {
            var usuario = _context.Usuarios.SingleOrDefault(u => u.Correo == userToken.Correo);

            if (usuario != null && usuario.Contrasena == userToken.Contrasena)
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userToken.Correo));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string tokencreado = tokenHandler.WriteToken(tokenConfig);

                return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
        }
    }
}