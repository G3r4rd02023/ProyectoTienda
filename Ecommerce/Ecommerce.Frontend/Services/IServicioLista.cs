using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Frontend.Services
{
    public interface IServicioLista
    {
        Task<IEnumerable<SelectListItem>> GetListaRoles();
    }
}