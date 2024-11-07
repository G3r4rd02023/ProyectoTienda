using Ecommerce.Frontend.Services;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Frontend.Models
{
    public class HomeViewModel
    {
        public int Cantidad { get; set; }

        public ICollection<Producto>? Productos { get; set; }

        public ICollection<Categoria>? Categorias { get; set; }
    }
}