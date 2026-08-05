using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificacionsController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public NotificacionsController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Notificacions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notificacion>>>
        GetNotificacions()
    {
        return await _context.Notificacions
            .AsNoTracking()
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();
    }

    // GET: api/Notificacions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Notificacion>>
        GetNotificacion(int id)
    {
        var notificacion =
            await _context.Notificacions.FindAsync(id);

        if (notificacion == null)
        {
            return NotFound();
        }

        return notificacion;
    }

    // POST: api/Notificacions
    [HttpPost]
    public async Task<ActionResult<Notificacion>>
        PostNotificacion(Notificacion notificacion)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u =>
                u.IdUsuario == notificacion.IdUsuario);

        if (!usuarioExiste)
        {
            return BadRequest("El usuario no existe.");
        }

        notificacion.IdNotificacion = 0;
        notificacion.FechaCreacion = DateTime.Now;
        notificacion.Leida = false;

        _context.Notificacions.Add(notificacion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetNotificacion),
            new { id = notificacion.IdNotificacion },
            notificacion
        );
    }

    // PUT: api/Notificacions/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutNotificacion(
        int id,
        Notificacion notificacion)
    {
        var notificacionActual =
            await _context.Notificacions.FindAsync(id);

        if (notificacionActual == null)
        {
            return NotFound();
        }

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u =>
                u.IdUsuario == notificacion.IdUsuario);

        if (!usuarioExiste)
        {
            return BadRequest("El usuario no existe.");
        }

        notificacionActual.IdUsuario =
            notificacion.IdUsuario;

        notificacionActual.Titulo =
            notificacion.Titulo;

        notificacionActual.Mensaje =
            notificacion.Mensaje;

        notificacionActual.Tipo =
            notificacion.Tipo;

        notificacionActual.Leida =
            notificacion.Leida;

        notificacionActual.Estado =
            notificacion.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/Notificacions/5/marcar-leida
    [HttpPut("{id}/marcar-leida")]
    public async Task<IActionResult> MarcarComoLeida(int id)
    {
        var notificacion =
            await _context.Notificacions.FindAsync(id);

        if (notificacion == null)
        {
            return NotFound();
        }

        notificacion.Leida = true;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Notificacions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotificacion(int id)
    {
        var notificacion =
            await _context.Notificacions.FindAsync(id);

        if (notificacion == null)
        {
            return NotFound();
        }

        _context.Notificacions.Remove(notificacion);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}