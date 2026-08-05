using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DetallePedidosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public DetallePedidosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/DetallePedidos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetallePedido>>>
        GetDetallePedidos()
    {
        return await _context.DetallePedidos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/DetallePedidos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetallePedido>>
        GetDetallePedido(int id)
    {
        var detalle = await _context.DetallePedidos.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        return detalle;
    }

    // POST: api/DetallePedidos
    [HttpPost]
    public async Task<ActionResult<DetallePedido>>
        PostDetallePedido(DetallePedido detalle)
    {
        detalle.IdDetallePedido = 0;

        _context.DetallePedidos.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDetallePedido),
            new { id = detalle.IdDetallePedido },
            detalle
        );
    }

    // PUT: api/DetallePedidos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetallePedido(
        int id,
        DetallePedido detalle)
    {
        var detalleActual =
            await _context.DetallePedidos.FindAsync(id);

        if (detalleActual == null)
        {
            return NotFound();
        }

        detalleActual.IdPedido = detalle.IdPedido;
        detalleActual.IdProducto = detalle.IdProducto;
        detalleActual.Cantidad = detalle.Cantidad;
        detalleActual.PrecioUnitario = detalle.PrecioUnitario;
        detalleActual.Descuento = detalle.Descuento;
        detalleActual.Impuesto = detalle.Impuesto;
        detalleActual.Subtotal = detalle.Subtotal;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DetallePedidos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetallePedido(int id)
    {
        var detalle = await _context.DetallePedidos.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        _context.DetallePedidos.Remove(detalle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
