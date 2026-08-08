using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class DetalleCarritosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public DetalleCarritosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/DetalleCarritos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleCarrito>>>
        GetDetalleCarritos()
    {
        return await _context.DetalleCarritos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/DetalleCarritos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetalleCarrito>>
        GetDetalleCarrito(int id)
    {
        var detalle = await _context.DetalleCarritos.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        return detalle;
    }

    // POST: api/DetalleCarritos
    [HttpPost]
    public async Task<ActionResult<DetalleCarrito>>
        PostDetalleCarrito(DetalleCarrito detalle)
    {
        detalle.IdDetalleCarrito = 0;

        _context.DetalleCarritos.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDetalleCarrito),
            new { id = detalle.IdDetalleCarrito },
            detalle
        );
    }

    // PUT: api/DetalleCarritos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalleCarrito(
        int id,
        DetalleCarrito detalle)
    {
        var detalleActual =
            await _context.DetalleCarritos.FindAsync(id);

        if (detalleActual == null)
        {
            return NotFound();
        }

        detalleActual.IdCarrito = detalle.IdCarrito;
        detalleActual.IdProducto = detalle.IdProducto;
        detalleActual.Cantidad = detalle.Cantidad;
        detalleActual.PrecioUnitario = detalle.PrecioUnitario;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DetalleCarritos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetalleCarrito(int id)
    {
        var detalle = await _context.DetalleCarritos.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        _context.DetalleCarritos.Remove(detalle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
