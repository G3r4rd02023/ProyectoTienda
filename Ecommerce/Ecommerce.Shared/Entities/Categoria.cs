using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;
    }
}