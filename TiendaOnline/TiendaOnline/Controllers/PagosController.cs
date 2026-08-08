using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public PagosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Pagos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
    {
        return await _context.Pagos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Pagos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Pago>> GetPago(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);

        if (pago == null)
        {
            return NotFound();
        }

        return pago;
    }

    // POST: api/Pagos
    [HttpPost]
    public async Task<ActionResult<Pago>> PostPago(Pago pago)
    {
        pago.IdPago = 0;

        if (string.IsNullOrWhiteSpace(pago.Estado))
        {
            pago.Estado = "Pendiente";
        }

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPago),
            new { id = pago.IdPago },
            pago
        );
    }

    // PUT: api/Pagos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPago(int id, Pago pago)
    {
        var pagoActual = await _context.Pagos.FindAsync(id);

        if (pagoActual == null)
        {
            return NotFound();
        }

        pagoActual.IdPedido = pago.IdPedido;
        pagoActual.MetodoPago = pago.MetodoPago;
        pagoActual.Referencia = pago.Referencia;
        pagoActual.Monto = pago.Monto;
        pagoActual.FechaPago = pago.FechaPago;
        pagoActual.Estado = pago.Estado;
        pagoActual.IdMetodoPago = pago.IdMetodoPago;
        pagoActual.IdEstadoPago = pago.IdEstadoPago;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Pagos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePago(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);

        if (pago == null)
        {
            return NotFound();
        }

        _context.Pagos.Remove(pago);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}