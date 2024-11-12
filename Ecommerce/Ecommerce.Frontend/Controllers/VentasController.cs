using Ecommerce.Frontend.Services;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Frontend.Controllers
{
    public class VentasController : Controller
    {
        private readonly IServicioVenta _venta;

        public VentasController(IServicioVenta venta)
        {
            _venta = venta;
        }

        public async Task<IActionResult> Index()
        {
            var venta = await _venta.ObtenerVentasAsync();
            return View(venta);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var venta = await _venta.ObtenerVentasAsync();
            var detalle = venta.FirstOrDefault(v => v.Id == id);
            return View(detalle);
        }

        public async Task<IActionResult> Send(int id)
        {
            Venta venta = await _venta.ObtenerVentaAsync(id);
            if (venta.EstadoPedido != EstadoPedido.Nuevo)
            {
                TempData["ErrorMessage"] = "Solo se pueden enviar pedidos que estén en estado 'nuevo'.";
            }
            else
            {
                venta.EstadoPedido = EstadoPedido.Enviado;

                await _venta.ActualizarVentaAsync(venta);
                TempData["SuccessMessage"] = "El estado del pedido ha sido cambiado a 'enviado'.";
            }
            return RedirectToAction(nameof(Index), new { venta.Id });
        }

        public async Task<IActionResult> Confirm(int id)
        {
            Venta venta = await _venta.ObtenerVentaAsync(id);
            if (venta.EstadoPedido != EstadoPedido.Enviado)
            {
                TempData["ErrorMessage"] = "Solo se pueden enviar pedidos que estén en estado 'enviado'.";
            }
            else
            {
                venta.EstadoPedido = EstadoPedido.Confirmado;

                await _venta.ActualizarVentaAsync(venta);
                TempData["SuccessMessage"] = "El estado del pedido ha sido cambiado a 'entregado'.";
            }
            return RedirectToAction(nameof(Index), new { venta.Id });
        }

        public async Task<IActionResult> Cancel(int id)
        {
            Venta venta = await _venta.ObtenerVentaAsync(id);
            if (venta.EstadoPedido == EstadoPedido.Cancelado)
            {
                TempData["ErrorMessage"] = "No se puede cancelar un pedido que esté en estado 'cancelado'.";
            }
            else
            {
                await _venta.CancelarVenta(venta.Id);
                TempData["ErrorMessage"] = "El estado del pedido ha sido cambiado a 'cancelado'.";
            }
            return RedirectToAction(nameof(Index), new { venta.Id });
        }
    }
}