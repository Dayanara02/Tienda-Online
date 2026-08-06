using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class BitacoraSistemasController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public BitacoraSistemasController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/BitacoraSistemas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BitacoraSistema>>>
        GetBitacoraSistemas()
    {
        return await _context.BitacoraSistemas
            .AsNoTracking()
            .OrderByDescending(b => b.Fecha)
            .ToListAsync();
    }

    // GET: api/BitacoraSistemas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BitacoraSistema>>
        GetBitacoraSistema(int id)
    {
        var bitacora =
            await _context.BitacoraSistemas.FindAsync(id);

        if (bitacora == null)
        {
            return NotFound();
        }

        return bitacora;
    }

    // POST: api/BitacoraSistemas
    [HttpPost]
    public async Task<ActionResult<BitacoraSistema>>
        PostBitacoraSistema(
            BitacoraSistema bitacora)
    {
        bitacora.IdBitacora = 0;
        bitacora.Fecha = DateTime.Now;

        _context.BitacoraSistemas.Add(bitacora);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetBitacoraSistema),
            new { id = bitacora.IdBitacora },
            bitacora
        );
    }
}