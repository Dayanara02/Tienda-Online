using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaDescuentosController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public CategoriaDescuentosController(TiendaOnlineContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaDescuentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCategoriaDescuentos()
        {
            var datos = await _context.Categoria
                .Include(c => c.IdDescuentos)
                .SelectMany(c => c.IdDescuentos.Select(d => new
                {
                    IdCategoria = c.IdCategoria,
                    IdDescuento = d.IdDescuento
                }))
                .ToListAsync();

            return Ok(datos);
        }

        // GET: api/CategoriaDescuentos/1/2
        [HttpGet("{idCategoria}/{idDescuento}")]
        public async Task<ActionResult<object>> GetCategoriaDescuento(
            int idCategoria,
            int idDescuento)
        {
            var categoria = await _context.Categoria
                .Include(c => c.IdDescuentos)
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);

            if (categoria == null)
                return NotFound();

            var existe = categoria.IdDescuentos
                .Any(d => d.IdDescuento == idDescuento);

            if (!existe)
                return NotFound();

            return Ok(new
            {
                IdCategoria = idCategoria,
                IdDescuento = idDescuento
            });
        }


        // POST: api/CategoriaDescuentos
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> PostCategoriaDescuento(
            [FromBody] CategoriaDescuentoRequest request)
        {
            var categoria = await _context.Categoria
                .Include(c => c.IdDescuentos)
                .FirstOrDefaultAsync(c => c.IdCategoria == request.IdCategoria);

            var descuento = await _context.Descuentos
                .FindAsync(request.IdDescuento);

            if (categoria == null || descuento == null)
                return NotFound("La categoría o el descuento no existe.");

            if (categoria.IdDescuentos.Any(d => d.IdDescuento == request.IdDescuento))
                return Conflict("La relación ya existe.");

            categoria.IdDescuentos.Add(descuento);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Descuento asociado correctamente.",
                idCategoria = request.IdCategoria,
                idDescuento = request.IdDescuento
            });

        }

        // DELETE: api/CategoriaDescuentos/1/2
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{idCategoria}/{idDescuento}")]
        public async Task<IActionResult> DeleteCategoriaDescuento(
            int idCategoria,
            int idDescuento)
        {
            var categoria = await _context.Categoria
                .Include(c => c.IdDescuentos)
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);

            if (categoria == null)
                return NotFound();

            var descuento = categoria.IdDescuentos
                .FirstOrDefault(d => d.IdDescuento == idDescuento);

            if (descuento == null)
                return NotFound();

            categoria.IdDescuentos.Remove(descuento);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        public class CategoriaDescuentoRequest
        {
            public int IdCategoria { get; set; }
            public int IdDescuento { get; set; }
        }
    }
}
