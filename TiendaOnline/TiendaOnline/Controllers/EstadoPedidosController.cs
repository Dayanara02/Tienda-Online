using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

// Solo usuarios con rol Administrador.
[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class EstadoPedidosController : ControllerBase
{
    // Contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Inyección del contexto.
    public EstadoPedidosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/EstadoPedidos
    // Consulta todos los estados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoPedido>>> GetEstadoPedidos()
    {
        return await _context.EstadoPedidos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/EstadoPedidos/5
    // Consulta un estado por ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<EstadoPedido>> GetEstadoPedido(int id)
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
    // Registra un nuevo estado.
    [HttpPost]
    public async Task<ActionResult<EstadoPedido>> PostEstadoPedido(
        EstadoPedido estadoPedido)
    {
        // La base de datos genera el ID.
        estadoPedido.IdEstadoPedido = 0;

        // Evita nombres duplicados.
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
    // Actualiza un estado existente.
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

        // Evita nombres repetidos.
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

        // Actualiza los datos.
        estadoActual.Nombre = estadoPedido.Nombre;
        estadoActual.Descripcion = estadoPedido.Descripcion;
        estadoActual.Estado = estadoPedido.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/EstadoPedidos/5
    // Elimina un estado existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstadoPedido(int id)
    {
        var estadoPedido =
            await _context.EstadoPedidos.FindAsync(id);

        if (estadoPedido == null)
        {
            return NotFound();
        }

        // Comprueba si el estado está siendo utilizado.
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