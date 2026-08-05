using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventariosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public InventariosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Inventarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inventario>>> GetInventarios()
    {
        return await _context.Inventarios
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Inventarios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Inventario>> GetInventario(int id)
    {
        var inventario = await _context.Inventarios.FindAsync(id);

        if (inventario == null)
        {
            return NotFound();
        }

        return inventario;
    }

    // POST: api/Inventarios
    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<Inventario>> PostInventario(
        Inventario inventario)
    {
        inventario.IdInventario = 0;
        inventario.FechaActualizacion = DateTime.Now;

        _context.Inventarios.Add(inventario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetInventario),
            new { id = inventario.IdInventario },
            inventario
        );
    }

    // PUT: api/Inventarios/5
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInventario(
        int id,
        Inventario inventario)
    {
        var inventarioActual = await _context.Inventarios.FindAsync(id);

        if (inventarioActual == null)
        {
            return NotFound();
        }

        inventarioActual.IdProducto = inventario.IdProducto;
        inventarioActual.CantidadDisponible =
            inventario.CantidadDisponible;
        inventarioActual.FechaActualizacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Inventarios/5
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventario(int id)
    {
        var inventario = await _context.Inventarios.FindAsync(id);

        if (inventario == null)
        {
            return NotFound();
        }

        _context.Inventarios.Remove(inventario);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
