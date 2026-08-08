using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ListaDeseosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public ListaDeseosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/ListaDeseos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListaDeseo>>> GetListaDeseos()
    {
        return await _context.ListaDeseos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/ListaDeseos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ListaDeseo>> GetListaDeseo(int id)
    {
        var listaDeseo = await _context.ListaDeseos.FindAsync(id);

        if (listaDeseo == null)
        {
            return NotFound();
        }

        return listaDeseo;
    }

    // POST: api/ListaDeseos
    [HttpPost]
    public async Task<ActionResult<ListaDeseo>> PostListaDeseo(
        ListaDeseo listaDeseo)
    {
        listaDeseo.IdListaDeseos = 0;
        listaDeseo.FechaCreacion = DateTime.Now;

        _context.ListaDeseos.Add(listaDeseo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetListaDeseo),
            new { id = listaDeseo.IdListaDeseos },
            listaDeseo
        );
    }

    // PUT: api/ListaDeseos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutListaDeseo(
        int id,
        ListaDeseo listaDeseo)
    {
        var listaActual = await _context.ListaDeseos.FindAsync(id);

        if (listaActual == null)
        {
            return NotFound();
        }

        listaActual.IdUsuario = listaDeseo.IdUsuario;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/ListaDeseos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteListaDeseo(int id)
    {
        var listaDeseo = await _context.ListaDeseos.FindAsync(id);

        if (listaDeseo == null)
        {
            return NotFound();
        }

        _context.ListaDeseos.Remove(listaDeseo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}