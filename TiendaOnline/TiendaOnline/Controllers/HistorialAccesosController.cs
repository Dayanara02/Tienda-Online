using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAccesosController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public HistorialAccesosController(TiendaOnlineContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialAcceso>>> GetHistorialAccesos()
        {
            return await _context.HistorialAccesos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialAcceso>> GetHistorialAcceso(int id)
        {
            var historial = await _context.HistorialAccesos.FindAsync(id);

            if (historial == null)
                return NotFound();

            return historial;
        }

        [HttpPost]
        public async Task<ActionResult<HistorialAcceso>> PostHistorialAcceso(
            HistorialAcceso historial)
        {
            historial.IdHistorialAcceso = 0;

            _context.HistorialAccesos.Add(historial);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetHistorialAcceso),
                new { id = historial.IdHistorialAcceso },
                historial
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorialAcceso(
            int id,
            HistorialAcceso historial)
        {
            if (id != historial.IdHistorialAcceso)
                return BadRequest();

            var existente = await _context.HistorialAccesos.FindAsync(id);

            if (existente == null)
                return NotFound();

            existente.IdUsuario = historial.IdUsuario;
            existente.FechaAcceso = historial.FechaAcceso;
            existente.DireccionIp = historial.DireccionIp;
            existente.Exitoso = historial.Exitoso;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorialAcceso(int id)
        {
            var historial = await _context.HistorialAccesos.FindAsync(id);

            if (historial == null)
                return NotFound();

            _context.HistorialAccesos.Remove(historial);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}