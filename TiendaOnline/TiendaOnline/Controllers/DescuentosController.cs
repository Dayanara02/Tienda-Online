using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescuentosController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public DescuentosController(TiendaOnlineContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Descuento>>> GetDescuentos()
        {
            return await _context.Descuentos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Descuento>> GetDescuento(int id)
        {
            var descuento = await _context.Descuentos.FindAsync(id);

            if (descuento == null)
                return NotFound();

            return descuento;
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Descuento>> PostDescuento(Descuento descuento)
        {
            descuento.IdDescuento = 0;

            _context.Descuentos.Add(descuento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDescuento),
                new { id = descuento.IdDescuento },
                descuento
            );
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDescuento(
            int id,
            Descuento descuento)
        {
            if (id != descuento.IdDescuento)
                return BadRequest();

            var existente = await _context.Descuentos.FindAsync(id);

            if (existente == null)
                return NotFound();

            existente.Nombre = descuento.Nombre;
            existente.Descripcion = descuento.Descripcion;
            existente.Porcentaje = descuento.Porcentaje;
            existente.FechaInicio = descuento.FechaInicio;
            existente.FechaFin = descuento.FechaFin;
            existente.Estado = descuento.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDescuento(int id)
        {
            var descuento = await _context.Descuentos.FindAsync(id);

            if (descuento == null)
                return NotFound();

            _context.Descuentos.Remove(descuento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}