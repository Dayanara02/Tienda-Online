using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoDescuentosController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public ProductoDescuentosController(TiendaOnlineContext context)
        {
            _context = context;
        }

        // GET: api/ProductoDescuentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProductoDescuentos()
        {
            var datos = await _context.Productos
                .Include(p => p.IdDescuentos)
                .SelectMany(p => p.IdDescuentos.Select(d => new
                {
                    IdProducto = p.IdProducto,
                    IdDescuento = d.IdDescuento
                }))
                .ToListAsync();

            return Ok(datos);
        }

        // GET: api/ProductoDescuentos/1/2
        [HttpGet("{idProducto}/{idDescuento}")]
        public async Task<ActionResult<object>> GetProductoDescuento(
            int idProducto,
            int idDescuento)
        {
            var producto = await _context.Productos
                .Include(p => p.IdDescuentos)
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            if (producto == null)
                return NotFound();

            var existe = producto.IdDescuentos
                .Any(d => d.IdDescuento == idDescuento);

            if (!existe)
                return NotFound();

            return Ok(new
            {
                IdProducto = idProducto,
                IdDescuento = idDescuento
            });
        }

        // POST: api/ProductoDescuentos
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostProductoDescuento(
            int idProducto,
            int idDescuento)
        {
            var producto = await _context.Productos
                .Include(p => p.IdDescuentos)
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            var descuento = await _context.Descuentos
                .FindAsync(idDescuento);

            if (producto == null || descuento == null)
                return NotFound();

            if (producto.IdDescuentos.Any(d => d.IdDescuento == idDescuento))
                return Conflict("La relación ya existe.");

            producto.IdDescuentos.Add(descuento);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Descuento asociado correctamente.",
                idProducto,
                idDescuento
            });
        }

        // DELETE: api/ProductoDescuentos/1/2
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{idProducto}/{idDescuento}")]
        public async Task<IActionResult> DeleteProductoDescuento(
            int idProducto,
            int idDescuento)
        {
            var producto = await _context.Productos
                .Include(p => p.IdDescuentos)
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            if (producto == null)
                return NotFound();

            var descuento = producto.IdDescuentos
                .FirstOrDefault(d => d.IdDescuento == idDescuento);

            if (descuento == null)
                return NotFound();

            producto.IdDescuentos.Remove(descuento);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}