using Ecommerce.Backend.Data;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductosController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Producto producto)
        {
            try
            {
                _context.Add(producto);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var producto = await _context.Productos
                .SingleOrDefaultAsync(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            try
            {
                _context.Update(producto);
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
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            _context.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("Resumen/")]
        public async Task<IActionResult> Resumen()
        {
            var lista = await _context.DetalleVentas
                    .GroupBy(dv => dv.Producto!.Nombre)
                    .OrderByDescending(grupo => grupo.Count())
                    .Take(5)
                    .Select(grupo => new ProductosViewModel
                    {
                        Producto = grupo.Key,
                        Cantidad = grupo.Count()
                    })
                    .ToListAsync();

            return Ok(lista);
        }
    }
}