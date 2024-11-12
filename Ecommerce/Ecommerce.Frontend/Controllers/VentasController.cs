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
        private readonly IServicioUsuario _usuario;

        public VentasController(IServicioVenta venta, IServicioUsuario usuario)
        {
            _venta = venta;
            _usuario = usuario;
        }

        public async Task<IActionResult> Index()
        {
            var venta = await _venta.ObtenerVentasAsync();
            return View(venta);
        }

        public async Task<IActionResult> Compras()
        {
            var ventas = await _venta.ObtenerVentasAsync();
            var compras = ventas.Where(s => s.Usuario!.Correo == User!.Identity!.Name).ToList();

            var usuarios = await _usuario.ObtenerUsuariosAsync();
            var usuario = usuarios.Where(u => u.Correo == User.Identity!.Name).FirstOrDefault();

            ViewBag.UsuarioActual = usuario;

            return View(compras);
        }

        public async Task<IActionResult> MisDetalles(int id)
        {
            var ventas = await _venta.ObtenerVentasAsync();
            var venta = ventas.Where(x => x.Usuario!.Correo == User.Identity!.Name).FirstOrDefault(v => v.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

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