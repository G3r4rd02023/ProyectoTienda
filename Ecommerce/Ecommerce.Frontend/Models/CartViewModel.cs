using Ecommerce.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Frontend.Models
{
    public class CartViewModel
    {
        public Usuario? Usuario { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Comentario { get; set; }

        public ICollection<VentaTemporal>? VentasTemporales { get; set; }

        public int Cantidad => VentasTemporales == null ? 0 : VentasTemporales.Sum(ts => ts.Cantidad);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Total => VentasTemporales == null ? 0 : VentasTemporales.Sum(ts => ts.Total);
    }
}