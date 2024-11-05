using Ecommerce.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Models
{
    public class AllowUserViewModel
    {
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(256, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Correo { get; set; } = null!;

        public Rol Rol { get; set; }

        public string Estado { get; set; } = null!;
    }
}