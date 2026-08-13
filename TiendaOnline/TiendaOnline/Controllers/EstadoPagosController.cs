using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;
// Indica que solamente los usuarios que tengan el rol
// "Administrador" pueden acceder a este controlador.
[Authorize(Roles = "Administrador")]
[ApiController]
// Define la ruta principal del controlador.
// [controller] será reemplazado por "EstadoPagos"
[Route("api/[controller]")]
public class EstadoPagosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public EstadoPagosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/EstadoPagos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoPago>>>
        GetEstadoPagos()
    {
        return await _context.EstadoPagos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/EstadoPagos/5
    // Permite eliminar un estado de pago
    [HttpGet("{id}")]
    public async Task<ActionResult<EstadoPago>>
        GetEstadoPago(int id)
    {
        var estadoPago =
            await _context.EstadoPagos.FindAsync(id);

        if (estadoPago == null)
        {
            return NotFound();
        }

        return estadoPago;
    }

    // POST: api/EstadoPagos
    [HttpPost]
    public async Task<ActionResult<EstadoPago>>
        PostEstadoPago(EstadoPago estadoPago)
    {
        estadoPago.IdEstadoPago = 0;

        var existe = await _context.EstadoPagos
            .AnyAsync(e => e.Nombre == estadoPago.Nombre);

        if (existe)
        {
            return Conflict(
                "Ya existe un estado de pago con ese nombre."
            );
        }

        _context.EstadoPagos.Add(estadoPago);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEstadoPago),
            new { id = estadoPago.IdEstadoPago },
            estadoPago
        );
    }

    // PUT: api/EstadoPagos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstadoPago(
        int id,
        EstadoPago estadoPago)
    {
        var estadoActual =
            await _context.EstadoPagos.FindAsync(id);

        if (estadoActual == null)
        {
            return NotFound();
        }

        var nombreExiste = await _context.EstadoPagos
            .AnyAsync(e =>
                e.Nombre == estadoPago.Nombre &&
                e.IdEstadoPago != id);

        if (nombreExiste)
        {
            return Conflict(
                "Ya existe otro estado de pago con ese nombre."
            );
        }

        estadoActual.Nombre = estadoPago.Nombre;
        estadoActual.Descripcion = estadoPago.Descripcion;
        estadoActual.Estado = estadoPago.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/EstadoPagos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstadoPago(int id)
    {
        var estadoPago =
            await _context.EstadoPagos.FindAsync(id);

        if (estadoPago == null)
        {
            return NotFound();
        }
        // Comprueba si existen pagos relacionados con el estado de pago que se intenta eliminar
        var tienePagos = await _context.Pagos
            .AnyAsync(p => p.IdEstadoPago == id);

        if (tienePagos)
        {
            return Conflict(
                "No se puede eliminar porque existen pagos relacionados."
            );
        }
        
        _context.EstadoPagos.Remove(estadoPago);
        // Guarda la eliminación en la base de datos
        await _context.SaveChangesAsync();
        // que la eliminación se realizó correctamente
        return NoContent();
    }
}