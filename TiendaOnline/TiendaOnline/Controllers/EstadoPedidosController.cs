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
[Route("api/[controller]")]
public class EstadoPedidosController : ControllerBase
{   // Variable privada que permite acceder a la base de datos
    // mediante Entity Framework Core.
    private readonly TiendaOnlineContext _context;

    public EstadoPedidosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/EstadoPedidos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoPedido>>>
        GetEstadoPedidos()
    {    // Consulta todos los registros de la tabla EstadoPedidos.
        // AsNoTracking() indica que los registros solamente serán
        // consultados y no modificados.
        // ToListAsync() ejecuta la consulta de forma asíncrona.
        return await _context.EstadoPedidos
            .AsNoTracking()
            .ToListAsync();
    }

    
    // GET: api/EstadoPedidos/5
    // Obtiene un estado de pedido específico utilizando su ID.
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
    {   // Se establece el ID en 0 para que la base de datos
        // genere automáticamente el identificador.
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
        // Actualiza el nombre del estado del pedido.
        estadoActual.Nombre = estadoPedido.Nombre;

        // Actualiza la descripción del estado.
        estadoActual.Descripcion = estadoPedido.Descripcion;

        // Actualiza el estado activo o inactivo.
        estadoActual.Estado = estadoPedido.Estado;

        // Guarda los cambios realizados en la base de datos.
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
        // que la eliminación se realizó correctamente.
        return NoContent();
    }
}
