using Ecommerce.Backend.Data;
using Ecommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporalesController : ControllerBase
    {
        private readonly DataContext _context;

        public TemporalesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.VentasTemporal
                .Include(vt => vt.Producto)
                .Include(vt => vt.Usuario)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(VentaTemporal temporal)
        {
            try
            {
                var producto = _context.Productos.FirstOrDefault(p => p.Id == temporal.Producto!.Id);
                var usuario = _context.Usuarios.FirstOrDefault(p => p.Id == temporal.Usuario!.Id);
                temporal.Usuario = usuario;
                temporal.Producto = producto;
                _context.Add(temporal);
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
        public async Task<IActionResult> PutAsync(int id, [FromBody] VentaTemporal venta)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var temporal = await _context.VentasTemporal.FindAsync(id);
            if (temporal == null)
            {
                return NotFound();
            }
            _context.Remove(temporal);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}