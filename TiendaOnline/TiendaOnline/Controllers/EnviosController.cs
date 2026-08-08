using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EnviosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public EnviosController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Envios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Envio>>>
        GetEnvios()
    {
        return await _context.Envios
            .AsNoTracking()
            .OrderByDescending(e => e.FechaEnvio)
            .ToListAsync();
    }

    // GET: api/Envios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Envio>>
        GetEnvio(int id)
    {
        var envio = await _context.Envios.FindAsync(id);

        if (envio == null)
        {
            return NotFound();
        }

        return envio;
    }

    // POST: api/Envios
    [HttpPost]
    public async Task<ActionResult<Envio>>
        PostEnvio(Envio envio)
    {
        var pedidoExiste = await _context.Pedidos
            .AnyAsync(p => p.IdPedido == envio.IdPedido);

        if (!pedidoExiste)
        {
            return BadRequest("El pedido no existe.");
        }

        var direccionExiste =
            await _context.DireccionUsuarios
                .AnyAsync(d =>
                    d.IdDireccion == envio.IdDireccion);

        if (!direccionExiste)
        {
            return BadRequest("La dirección no existe.");
        }

        var pedidoYaTieneEnvio = await _context.Envios
            .AnyAsync(e => e.IdPedido == envio.IdPedido);

        if (pedidoYaTieneEnvio)
        {
            return Conflict(
                "El pedido ya tiene un envío registrado."
            );
        }

        envio.IdEnvio = 0;

        if (string.IsNullOrWhiteSpace(envio.Estado))
        {
            envio.Estado = "Pendiente";
        }

        _context.Envios.Add(envio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEnvio),
            new { id = envio.IdEnvio },
            envio
        );
    }

    // PUT: api/Envios/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEnvio(
        int id,
        Envio envio)
    {
        var envioActual =
            await _context.Envios.FindAsync(id);

        if (envioActual == null)
        {
            return NotFound();
        }

        var pedidoExiste = await _context.Pedidos
            .AnyAsync(p => p.IdPedido == envio.IdPedido);

        if (!pedidoExiste)
        {
            return BadRequest("El pedido no existe.");
        }

        var direccionExiste =
            await _context.DireccionUsuarios
                .AnyAsync(d =>
                    d.IdDireccion == envio.IdDireccion);

        if (!direccionExiste)
        {
            return BadRequest("La dirección no existe.");
        }

        var otroEnvioDelPedido = await _context.Envios
            .AnyAsync(e =>
                e.IdPedido == envio.IdPedido &&
                e.IdEnvio != id);

        if (otroEnvioDelPedido)
        {
            return Conflict(
                "El pedido ya tiene otro envío registrado."
            );
        }

        envioActual.IdPedido = envio.IdPedido;
        envioActual.IdDireccion = envio.IdDireccion;
        envioActual.EmpresaEnvio = envio.EmpresaEnvio;
        envioActual.NumeroSeguimiento =
            envio.NumeroSeguimiento;
        envioActual.FechaEnvio = envio.FechaEnvio;
        envioActual.FechaEntrega = envio.FechaEntrega;
        envioActual.Estado = envio.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Envios/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnvio(int id)
    {
        var envio =
            await _context.Envios.FindAsync(id);

        if (envio == null)
        {
            return NotFound();
        }

        _context.Envios.Remove(envio);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}