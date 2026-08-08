using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MovimientoInventariosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public MovimientoInventariosController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/MovimientoInventarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoInventario>>>
        GetMovimientoInventarios()
    {
        return await _context.MovimientoInventarios
            .AsNoTracking()
            .OrderByDescending(m => m.FechaMovimiento)
            .ToListAsync();
    }

    // GET: api/MovimientoInventarios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MovimientoInventario>>
        GetMovimientoInventario(int id)
    {
        var movimiento =
            await _context.MovimientoInventarios.FindAsync(id);

        if (movimiento == null)
        {
            return NotFound();
        }

        return movimiento;
    }

    // POST: api/MovimientoInventarios
    [HttpPost]
    public async Task<ActionResult<MovimientoInventario>>
        PostMovimientoInventario(
            MovimientoInventario movimiento)
    {
        var inventario = await _context.Inventarios
            .FindAsync(movimiento.IdInventario);

        if (inventario == null)
        {
            return BadRequest("El inventario no existe.");
        }

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.IdUsuario == movimiento.IdUsuario);

        if (!usuarioExiste)
        {
            return BadRequest("El usuario no existe.");
        }

        var tiposPermitidos = new[]
        {
            "Entrada",
            "Salida",
            "Ajuste"
        };

        if (!tiposPermitidos.Contains(movimiento.TipoMovimiento))
        {
            return BadRequest(
                "El tipo de movimiento debe ser Entrada, Salida o Ajuste."
            );
        }

        if (movimiento.Cantidad <= 0)
        {
            return BadRequest(
                "La cantidad debe ser mayor que cero."
            );
        }

        if (movimiento.TipoMovimiento == "Salida")
        {
            if (inventario.CantidadDisponible <
                movimiento.Cantidad)
            {
                return BadRequest(
                    "No existe suficiente cantidad disponible."
                );
            }

            inventario.CantidadDisponible -=
                movimiento.Cantidad;
        }
        else if (movimiento.TipoMovimiento == "Entrada")
        {
            inventario.CantidadDisponible +=
                movimiento.Cantidad;
        }
        else if (movimiento.TipoMovimiento == "Ajuste")
        {
            inventario.CantidadDisponible =
                movimiento.Cantidad;
        }

        inventario.FechaActualizacion = DateTime.Now;

        movimiento.IdMovimiento = 0;
        movimiento.FechaMovimiento = DateTime.Now;

        _context.MovimientoInventarios.Add(movimiento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMovimientoInventario),
            new { id = movimiento.IdMovimiento },
            movimiento
        );
    }
}