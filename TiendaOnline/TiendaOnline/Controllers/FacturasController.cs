using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public FacturasController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Facturas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
    {
        return await _context.Facturas
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Facturas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Factura>> GetFactura(int id)
    {
        var factura = await _context.Facturas.FindAsync(id);

        if (factura == null)
        {
            return NotFound();
        }

        return factura;
    }

    // POST: api/Facturas
    [HttpPost]
    public async Task<ActionResult<Factura>> PostFactura(
        Factura factura)
    {
        factura.IdFactura = 0;
        factura.FechaEmision = DateTime.Now;

        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetFactura),
            new { id = factura.IdFactura },
            factura
        );
    }

    // PUT: api/Facturas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFactura(
        int id,
        Factura factura)
    {
        var facturaActual = await _context.Facturas.FindAsync(id);

        if (facturaActual == null)
        {
            return NotFound();
        }

        facturaActual.IdPedido = factura.IdPedido;
        facturaActual.NumeroFactura = factura.NumeroFactura;
        facturaActual.Subtotal = factura.Subtotal;
        facturaActual.Impuesto = factura.Impuesto;
        facturaActual.Descuento = factura.Descuento;
        facturaActual.Total = factura.Total;
        facturaActual.UrlPdf = factura.UrlPdf;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Facturas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFactura(int id)
    {
        var factura = await _context.Facturas.FindAsync(id);

        if (factura == null)
        {
            return NotFound();
        }

        _context.Facturas.Remove(factura);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}