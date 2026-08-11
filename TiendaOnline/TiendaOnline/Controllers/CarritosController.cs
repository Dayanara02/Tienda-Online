using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarritosController : ControllerBase
{
    // Tu código actual

    private readonly TiendaOnlineContext _context;

    public CarritosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Carritos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Carrito>>> GetCarritos()
    {
        return await _context.Carritos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Carritos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Carrito>> GetCarrito(int id)
    {
        var carrito = await _context.Carritos.FindAsync(id);

        if (carrito == null)
        {
            return NotFound();
        }

        return carrito;
    }

    // POST: api/Carritos
    [HttpPost]
    public async Task<ActionResult<Carrito>> PostCarrito(
        Carrito carrito)
    {
        carrito.IdCarrito = 0;
        carrito.FechaCreacion = DateTime.Now;

        if (string.IsNullOrWhiteSpace(carrito.Estado))
        {
            carrito.Estado = "Activo";
        }

        _context.Carritos.Add(carrito);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCarrito),
            new { id = carrito.IdCarrito },
            carrito
        );
    }

    // PUT: api/Carritos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCarrito(
        int id,
        Carrito carrito)
    {
        var carritoActual = await _context.Carritos.FindAsync(id);

        if (carritoActual == null)
        {
            return NotFound();
        }

        carritoActual.IdUsuario = carrito.IdUsuario;
        carritoActual.Estado = carrito.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Carritos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCarrito(int id)
    {
        var carrito = await _context.Carritos.FindAsync(id);

        if (carrito == null)
        {
            return NotFound();
        }

        _context.Carritos.Remove(carrito);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}