using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DetalleProformasController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public DetalleProformasController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/DetalleProformas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleProforma>>>
        GetDetalleProformas()
    {
        return await _context.DetalleProformas
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/DetalleProformas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetalleProforma>>
        GetDetalleProforma(int id)
    {
        var detalle =
            await _context.DetalleProformas.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        return detalle;
    }

    // POST: api/DetalleProformas
    [HttpPost]
    public async Task<ActionResult<DetalleProforma>>
        PostDetalleProforma(DetalleProforma detalle)
    {
        detalle.IdDetalleProforma = 0;

        var proformaExiste = await _context.Proformas
            .AnyAsync(p => p.IdProforma == detalle.IdProforma);

        if (!proformaExiste)
        {
            return BadRequest("La proforma no existe.");
        }

        var productoExiste = await _context.Productos
            .AnyAsync(p => p.IdProducto == detalle.IdProducto);

        if (!productoExiste)
        {
            return BadRequest("El producto no existe.");
        }

        _context.DetalleProformas.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDetalleProforma),
            new { id = detalle.IdDetalleProforma },
            detalle
        );
    }

    // PUT: api/DetalleProformas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalleProforma(
        int id,
        DetalleProforma detalle)
    {
        var detalleActual =
            await _context.DetalleProformas.FindAsync(id);

        if (detalleActual == null)
        {
            return NotFound();
        }

        detalleActual.IdProforma = detalle.IdProforma;
        detalleActual.IdProducto = detalle.IdProducto;
        detalleActual.Cantidad = detalle.Cantidad;
        detalleActual.PrecioUnitario =
            detalle.PrecioUnitario;
        detalleActual.Descuento = detalle.Descuento;
        detalleActual.Impuesto = detalle.Impuesto;
        detalleActual.Subtotal = detalle.Subtotal;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DetalleProformas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetalleProforma(
        int id)
    {
        var detalle =
            await _context.DetalleProformas.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        _context.DetalleProformas.Remove(detalle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
