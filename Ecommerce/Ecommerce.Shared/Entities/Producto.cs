using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Shared.Entities
{
    public class Producto
    {
        public int Id { get; set; }

        [DisplayName("Código")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Codigo { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayName("Unidades disponibles")]
        public int Stock { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Descripcion { get; set; }

        [Display(Name = "Foto")]
        public string? URLFoto { get; set; }

        public Categoria? Categoria { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes de seleccionar una categoria.")]
        public int CategoriaId { get; set; }
    }
}