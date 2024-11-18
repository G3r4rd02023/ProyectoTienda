using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Models
{
    public class UsuarioToken
    {
        [MaxLength(256, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Correo { get; set; } = null!;

        [MaxLength(256, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Contrasena { get; set; } = null!;
    }
}