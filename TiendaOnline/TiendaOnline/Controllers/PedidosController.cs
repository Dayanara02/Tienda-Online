using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using TiendaOnline.LogicaNegocio.Interfaces;
using TiendaOnline.LogicaNegocio.Servicios;
using System.Security.Claims;
using TiendaOnline.Dominio.DTO;
using TiendaOnline.LogicaNegocio.Interfaces;

namespace TiendaOnline.API.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]

public class PedidosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;
    private readonly IPedidoServicio _pedidoServicio;

    public PedidosController(TiendaOnlineContext context, IPedidoServicio pedidoServicio)
    {
        _context = context;
        _pedidoServicio = pedidoServicio;
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

    // POST: api/Pedidos/confirmar
    [HttpPost("confirmar")]
    public async Task<ActionResult<PedidoCreadoDto>> ConfirmarPedido(
        PedidoCrearDto pedidoDto)
    {
        var idUsuarioTexto = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrWhiteSpace(idUsuarioTexto) ||
            !int.TryParse(idUsuarioTexto, out var idUsuario))
        {
            return Unauthorized(new
            {
                mensaje = "No se pudo identificar al usuario."
            });
        }

        var resultado = await _pedidoServicio.CrearPedidoAsync(
            idUsuario,
            pedidoDto
        );

        return Ok(resultado);
    }
}