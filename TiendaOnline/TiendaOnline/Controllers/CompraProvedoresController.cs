using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class CompraProveedorsController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public CompraProveedorsController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/CompraProveedors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompraProveedor>>>
        GetCompraProveedors()
    {
        return await _context.CompraProveedors
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/CompraProveedors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CompraProveedor>>
        GetCompraProveedor(int id)
    {
        var compra =
            await _context.CompraProveedors.FindAsync(id);

        if (compra == null)
        {
            return NotFound();
        }

        return compra;
    }

    // POST: api/CompraProveedors
    [HttpPost]
    public async Task<ActionResult<CompraProveedor>>
        PostCompraProveedor(CompraProveedor compra)
    {
        compra.IdCompraProveedor = 0;
        compra.FechaCompra = DateTime.Now;

        if (string.IsNullOrWhiteSpace(compra.Estado))
        {
            compra.Estado = "Pendiente";
        }

        _context.CompraProveedors.Add(compra);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCompraProveedor),
            new { id = compra.IdCompraProveedor },
            compra
        );
    }

    // PUT: api/CompraProveedors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCompraProveedor(
        int id,
        CompraProveedor compra)
    {
        var compraActual =
            await _context.CompraProveedors.FindAsync(id);

        if (compraActual == null)
        {
            return NotFound();
        }

        compraActual.IdProveedor = compra.IdProveedor;
        compraActual.IdUsuario = compra.IdUsuario;
        compraActual.Subtotal = compra.Subtotal;
        compraActual.Impuesto = compra.Impuesto;
        compraActual.Total = compra.Total;
        compraActual.Estado = compra.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/CompraProveedors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompraProveedor(int id)
    {
        var compra =
            await _context.CompraProveedors.FindAsync(id);

        if (compra == null)
        {
            return NotFound();
        }

        _context.CompraProveedors.Remove(compra);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}