using Ecommerce.Backend.Data;
using Ecommerce.Shared.Entities;
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
    }
}