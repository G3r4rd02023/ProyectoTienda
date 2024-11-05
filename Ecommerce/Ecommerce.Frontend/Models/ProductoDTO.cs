using Ecommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Frontend.Models
{
    public class ProductoDTO : Producto
    {
        [NotMapped]
        public IEnumerable<SelectListItem>? Categorias { get; set; }
    }
}