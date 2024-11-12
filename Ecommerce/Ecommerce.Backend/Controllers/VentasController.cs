using Ecommerce.Backend.Data;
using Ecommerce.Backend.Migrations;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly DataContext _context;

        public VentasController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.DetallesVenta!)
                .ThenInclude(dv => dv.Producto)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var venta = await _context.Ventas
                .SingleOrDefaultAsync(p => p.Id == id);
            if (venta == null)
            {
                return NotFound();
            }
            return Ok(venta);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Venta venta)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(p => p.Id == venta.Usuario!.Id);
                venta.Usuario = usuario;

                foreach (var detalle in venta.DetallesVenta!)
                {
                    var productoExistente = _context.Productos.FirstOrDefault(p => p.Id == detalle.Producto!.Id);
                    if (productoExistente != null)
                    {
                        detalle.Producto = productoExistente;
                    }
                }

                _context.Add(venta);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException!.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Venta venta)
        {
            if (id != venta.Id)
            {
                return BadRequest();
            }

            try
            {
                _context.Update(venta);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException!.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Resumen/")]
        public async Task<IActionResult> Resumen()
        {
            DateTime FechaInicio = DateTime.Now;
            FechaInicio = FechaInicio.AddDays(-5);

            var lista = await _context.Ventas
                        .Where(tbventa => tbventa.Fecha.Date >= FechaInicio.Date)
                        .GroupBy(tbventa => tbventa.Fecha.Date)
                        .Select(grupo => new VentasViewModel
                        {
                            Fecha = grupo.Key.ToString("dd/MM/yyyy"),
                            Cantidad = grupo.Count()
                        })
                        .ToListAsync();

            return Ok(lista);
        }
    }
}