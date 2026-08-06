using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class MetodoPagosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public MetodoPagosController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/MetodoPagos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MetodoPago>>>
        GetMetodoPagos()
    {
        return await _context.MetodoPagos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/MetodoPagos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MetodoPago>>
        GetMetodoPago(int id)
    {
        var metodoPago =
            await _context.MetodoPagos.FindAsync(id);

        if (metodoPago == null)
        {
            return NotFound();
        }

        return metodoPago;
    }

    // POST: api/MetodoPagos
    [HttpPost]
    public async Task<ActionResult<MetodoPago>>
        PostMetodoPago(
            MetodoPago metodoPago)
    {
        metodoPago.IdMetodoPago = 0;

        var existe = await _context.MetodoPagos
            .AnyAsync(m => m.Nombre == metodoPago.Nombre);

        if (existe)
        {
            return Conflict(
                "Ya existe un método de pago con ese nombre."
            );
        }

        _context.MetodoPagos.Add(metodoPago);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMetodoPago),
            new { id = metodoPago.IdMetodoPago },
            metodoPago
        );
    }

    // PUT: api/MetodoPagos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMetodoPago(
        int id,
        MetodoPago metodoPago)
    {
        var metodoActual =
            await _context.MetodoPagos.FindAsync(id);

        if (metodoActual == null)
        {
            return NotFound();
        }

        metodoActual.Nombre = metodoPago.Nombre;
        metodoActual.Descripcion =
            metodoPago.Descripcion;
        metodoActual.Estado = metodoPago.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/MetodoPagos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMetodoPago(int id)
    {
        var metodoPago =
            await _context.MetodoPagos.FindAsync(id);

        if (metodoPago == null)
        {
            return NotFound();
        }

        _context.MetodoPagos.Remove(metodoPago);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}