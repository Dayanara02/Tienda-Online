using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EvaluacionProductosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public EvaluacionProductosController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/EvaluacionProductos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EvaluacionProducto>>>
        GetEvaluacionProductos()
    {
        return await _context.EvaluacionProductos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/EvaluacionProductos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EvaluacionProducto>>
        GetEvaluacionProducto(int id)
    {
        var evaluacion =
            await _context.EvaluacionProductos.FindAsync(id);

        if (evaluacion == null)
        {
            return NotFound();
        }

        return evaluacion;
    }

    // POST: api/EvaluacionProductos
    [HttpPost]
    public async Task<ActionResult<EvaluacionProducto>>
        PostEvaluacionProducto(
            EvaluacionProducto evaluacion)
    {
        evaluacion.IdEvaluacion = 0;
        evaluacion.FechaEvaluacion = DateTime.Now;

        _context.EvaluacionProductos.Add(evaluacion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvaluacionProducto),
            new { id = evaluacion.IdEvaluacion },
            evaluacion
        );
    }

    // PUT: api/EvaluacionProductos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEvaluacionProducto(
        int id,
        EvaluacionProducto evaluacion)
    {
        var evaluacionActual =
            await _context.EvaluacionProductos.FindAsync(id);

        if (evaluacionActual == null)
        {
            return NotFound();
        }

        evaluacionActual.IdProducto =
            evaluacion.IdProducto;
        evaluacionActual.IdUsuario =
            evaluacion.IdUsuario;
        evaluacionActual.Calificacion =
            evaluacion.Calificacion;
        evaluacionActual.Comentario =
            evaluacion.Comentario;
        evaluacionActual.Estado =
            evaluacion.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/EvaluacionProductos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        DeleteEvaluacionProducto(int id)
    {
        var evaluacion =
            await _context.EvaluacionProductos.FindAsync(id);

        if (evaluacion == null)
        {
            return NotFound();
        }

        _context.EvaluacionProductos.Remove(evaluacion);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
