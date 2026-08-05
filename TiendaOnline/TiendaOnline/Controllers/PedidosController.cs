using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]

public class PedidosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public PedidosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Pedidos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
    {
        return await _context.Pedidos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Pedidos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> GetPedido(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
        {
            return NotFound();
        }

        return pedido;
    }

    // POST: api/Pedidos
    [HttpPost]
    public async Task<ActionResult<Pedido>> PostPedido(
        Pedido pedido)
    {
        pedido.IdPedido = 0;
        pedido.FechaPedido = DateTime.Now;

        if (string.IsNullOrWhiteSpace(pedido.Estado))
        {
            pedido.Estado = "Pendiente";
        }

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPedido),
            new { id = pedido.IdPedido },
            pedido
        );
    }

    // PUT: api/Pedidos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPedido(
        int id,
        Pedido pedido)
    {
        var pedidoActual = await _context.Pedidos.FindAsync(id);

        if (pedidoActual == null)
        {
            return NotFound();
        }

        pedidoActual.IdUsuario = pedido.IdUsuario;
        pedidoActual.Estado = pedido.Estado;
        pedidoActual.Subtotal = pedido.Subtotal;
        pedidoActual.Impuesto = pedido.Impuesto;
        pedidoActual.Descuento = pedido.Descuento;
        pedidoActual.Total = pedido.Total;
        pedidoActual.DireccionEntrega =
            pedido.DireccionEntrega;
        pedidoActual.IdEstadoPedido =
            pedido.IdEstadoPedido;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Pedidos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePedido(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
        {
            return NotFound();
        }

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}