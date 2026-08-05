using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Dominio.Model;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliaDescuentosController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public FamiliaDescuentosController(TiendaOnlineContext context)
        {
            _context = context;
        }

        // GET: api/FamiliaDescuentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFamiliaDescuentos()
        {
            var datos = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)
                .SelectMany(f => f.IdDescuentos.Select(d => new
                {
                    IdFamilia = f.IdFamilia,
                    IdDescuento = d.IdDescuento
                }))
                .ToListAsync();

            return Ok(datos);
        }

        // GET: api/FamiliaDescuentos/1/2
        [HttpGet("{idFamilia}/{idDescuento}")]
        public async Task<ActionResult<object>> GetFamiliaDescuento(
            int idFamilia,
            int idDescuento)
        {
            var familia = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)
                .FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            if (familia == null)
                return NotFound();

            var existe = familia.IdDescuentos
                .Any(d => d.IdDescuento == idDescuento);

            if (!existe)
                return NotFound();

            return Ok(new
            {
                IdFamilia = idFamilia,
                IdDescuento = idDescuento
            });
        }

        // POST: api/FamiliaDescuentos
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostFamiliaDescuento(
            int idFamilia,
            int idDescuento)
        {
            var familia = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)
                .FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            var descuento = await _context.Descuentos
                .FindAsync(idDescuento);

            if (familia == null || descuento == null)
                return NotFound();

            if (familia.IdDescuentos.Any(d => d.IdDescuento == idDescuento))
                return Conflict("La relación ya existe.");

            familia.IdDescuentos.Add(descuento);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Descuento asociado correctamente.",
                idFamilia,
                idDescuento
            });
        }

        // DELETE: api/FamiliaDescuentos/1/2
        [HttpDelete("{idFamilia}/{idDescuento}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteFamiliaDescuento(
            int idFamilia,
            int idDescuento)
        {
            var familia = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)
                .FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            if (familia == null)
                return NotFound();

            var descuento = familia.IdDescuentos
                .FirstOrDefault(d => d.IdDescuento == idDescuento);

            if (descuento == null)
                return NotFound();

            familia.IdDescuentos.Remove(descuento);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
