using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImpuestosController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public ImpuestosController(TiendaOnlineContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Impuesto>>> GetImpuestos()
        {
            return await _context.Impuestos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Impuesto>> GetImpuesto(int id)
        {
            var impuesto = await _context.Impuestos.FindAsync(id);

            if (impuesto == null)
                return NotFound();

            return impuesto;
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Impuesto>> PostImpuesto(
            Impuesto impuesto)
        {
            impuesto.IdImpuesto = 0;

            _context.Impuestos.Add(impuesto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetImpuesto),
                new { id = impuesto.IdImpuesto },
                impuesto
            );
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImpuesto(
            int id,
            Impuesto impuesto)
        {
            if (id != impuesto.IdImpuesto)
                return BadRequest();

            var existente = await _context.Impuestos.FindAsync(id);

            if (existente == null)
                return NotFound();

            existente.Nombre = impuesto.Nombre;
            existente.Porcentaje = impuesto.Porcentaje;
            existente.Estado = impuesto.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImpuesto(int id)
        {
            var impuesto = await _context.Impuestos.FindAsync(id);

            if (impuesto == null)
                return NotFound();

            _context.Impuestos.Remove(impuesto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
