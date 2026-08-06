using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class DetalleCompraProveedorsController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public DetalleCompraProveedorsController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/DetalleCompraProveedors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleCompraProveedor>>>
        GetDetalleCompraProveedors()
    {
        return await _context.DetalleCompraProveedors
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/DetalleCompraProveedors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetalleCompraProveedor>>
        GetDetalleCompraProveedor(int id)
    {
        var detalle =
            await _context.DetalleCompraProveedors.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        return detalle;
    }

    // POST: api/DetalleCompraProveedors
    [HttpPost]
    public async Task<ActionResult<DetalleCompraProveedor>>
        PostDetalleCompraProveedor(
            DetalleCompraProveedor detalle)
    {
        detalle.IdDetalleCompra = 0;

        _context.DetalleCompraProveedors.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDetalleCompraProveedor),
            new { id = detalle.IdDetalleCompra },
            detalle
        );
    }

    // PUT: api/DetalleCompraProveedors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalleCompraProveedor(
        int id,
        DetalleCompraProveedor detalle)
    {
        var detalleActual =
            await _context.DetalleCompraProveedors.FindAsync(id);

        if (detalleActual == null)
        {
            return NotFound();
        }

        detalleActual.IdCompraProveedor =
            detalle.IdCompraProveedor;
        detalleActual.IdProducto = detalle.IdProducto;
        detalleActual.Cantidad = detalle.Cantidad;
        detalleActual.PrecioUnitario =
            detalle.PrecioUnitario;
        detalleActual.Subtotal = detalle.Subtotal;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DetalleCompraProveedors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        DeleteDetalleCompraProveedor(int id)
    {
        var detalle =
            await _context.DetalleCompraProveedors.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        _context.DetalleCompraProveedors.Remove(detalle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
