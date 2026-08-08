using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class EstadoPedidosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public EstadoPedidosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/EstadoPedidos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoPedido>>>
        GetEstadoPedidos()
    {
        return await _context.EstadoPedidos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/EstadoPedidos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EstadoPedido>>
        GetEstadoPedido(int id)
    {
        var estadoPedido =
            await _context.EstadoPedidos.FindAsync(id);

        if (estadoPedido == null)
        {
            return NotFound();
        }

        return estadoPedido;
    }

    // POST: api/EstadoPedidos
    [HttpPost]
    public async Task<ActionResult<EstadoPedido>>
        PostEstadoPedido(EstadoPedido estadoPedido)
    {
        estadoPedido.IdEstadoPedido = 0;

        var existe = await _context.EstadoPedidos
            .AnyAsync(e => e.Nombre == estadoPedido.Nombre);

        if (existe)
        {
            return Conflict(
                "Ya existe un estado de pedido con ese nombre."
            );
        }

        _context.EstadoPedidos.Add(estadoPedido);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEstadoPedido),
            new { id = estadoPedido.IdEstadoPedido },
            estadoPedido
        );
    }

    // PUT: api/EstadoPedidos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstadoPedido(
        int id,
        EstadoPedido estadoPedido)
    {
        var estadoActual =
            await _context.EstadoPedidos.FindAsync(id);

        if (estadoActual == null)
        {
            return NotFound();
        }

        var nombreExiste = await _context.EstadoPedidos
            .AnyAsync(e =>
                e.Nombre == estadoPedido.Nombre &&
                e.IdEstadoPedido != id);

        if (nombreExiste)
        {
            return Conflict(
                "Ya existe otro estado de pedido con ese nombre."
            );
        }

        estadoActual.Nombre = estadoPedido.Nombre;
        estadoActual.Descripcion = estadoPedido.Descripcion;
        estadoActual.Estado = estadoPedido.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/EstadoPedidos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstadoPedido(int id)
    {
        var estadoPedido =
            await _context.EstadoPedidos.FindAsync(id);

        if (estadoPedido == null)
        {
            return NotFound();
        }

        var tienePedidos = await _context.Pedidos
            .AnyAsync(p => p.IdEstadoPedido == id);

        if (tienePedidos)
        {
            return Conflict(
                "No se puede eliminar porque existen pedidos relacionados."
            );
        }

        _context.EstadoPedidos.Remove(estadoPedido);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
