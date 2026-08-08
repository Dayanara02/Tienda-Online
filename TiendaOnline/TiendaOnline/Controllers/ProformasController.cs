using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProformasController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public ProformasController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Proformas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Proforma>>>
        GetProformas()
    {
        return await _context.Proformas
            .AsNoTracking()
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    // GET: api/Proformas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Proforma>>
        GetProforma(int id)
    {
        var proforma =
            await _context.Proformas.FindAsync(id);

        if (proforma == null)
        {
            return NotFound();
        }

        return proforma;
    }

    // POST: api/Proformas
    [HttpPost]
    public async Task<ActionResult<Proforma>>
        PostProforma(Proforma proforma)
    {
        proforma.IdProforma = 0;
        proforma.FechaCreacion = DateTime.Now;

        if (string.IsNullOrWhiteSpace(proforma.Estado))
        {
            proforma.Estado = "Pendiente";
        }

        _context.Proformas.Add(proforma);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProforma),
            new { id = proforma.IdProforma },
            proforma
        );
    }

    // PUT: api/Proformas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProforma(
        int id,
        Proforma proforma)
    {
        var proformaActual =
            await _context.Proformas.FindAsync(id);

        if (proformaActual == null)
        {
            return NotFound();
        }

        proformaActual.IdUsuario = proforma.IdUsuario;
        proformaActual.IdDireccion = proforma.IdDireccion;
        proformaActual.FechaVencimiento =
            proforma.FechaVencimiento;
        proformaActual.Subtotal = proforma.Subtotal;
        proformaActual.Impuesto = proforma.Impuesto;
        proformaActual.Descuento = proforma.Descuento;
        proformaActual.Total = proforma.Total;
        proformaActual.Estado = proforma.Estado;
        proformaActual.UrlPdf = proforma.UrlPdf;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Proformas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProforma(int id)
    {
        var proforma =
            await _context.Proformas.FindAsync(id);

        if (proforma == null)
        {
            return NotFound();
        }

        var detalles = await _context.DetalleProformas
            .Where(d => d.IdProforma == id)
            .ToListAsync();

        if (detalles.Count > 0)
        {
            _context.DetalleProformas.RemoveRange(detalles);
        }

        _context.Proformas.Remove(proforma);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}