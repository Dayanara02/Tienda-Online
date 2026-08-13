// Permite crear controladores API.
using Microsoft.AspNetCore.Mvc;

// Permite consultar la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto principal.
using TiendaOnline.AccesoDatos.Context;

// Permite proteger los endpoints.
using Microsoft.AspNetCore.Authorization;

// Importa las entidades.
using TiendaOnline.Dominio.Entidades;

// Permite leer datos del token.
using System.Security.Claims;

namespace TiendaOnline.API.Controllers;

// Requiere usuario autenticado.
[Authorize]

// Define un controlador API.
[ApiController]

// Define la ruta correcta.
[Route("api/Notificaciones")]
public class NotificacionsController : ControllerBase
{
    // Guarda el contexto.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto.
    public NotificacionsController(
        TiendaOnlineContext context)
    {
        // Guarda el contexto recibido.
        _context = context;
    }

    // Obtiene todas las notificaciones.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notificacion>>>
        GetNotificacions()
    {
        // Consulta las notificaciones.
        return await _context.Notificacions
            .AsNoTracking()
            .OrderByDescending(
                n => n.FechaCreacion
            )
            .ToListAsync();
    }

    // Obtiene las notificaciones del cliente.
    [Authorize(Roles = "Cliente")]
    [HttpGet("mis-notificaciones")]
    public async Task<IActionResult>
        GetMisNotificaciones()
    {
        // Obtiene el usuario del token.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        // Valida el usuario.
        if (
            string.IsNullOrWhiteSpace(
                idUsuarioTexto
            ) ||
            !int.TryParse(
                idUsuarioTexto,
                out var idUsuario
            )
        )
        {
            return Unauthorized(
                "No se pudo identificar al usuario."
            );
        }

        // Consulta solo las del usuario.
        var notificaciones =
            await _context.Notificacions
                .AsNoTracking()
                .Where(
                    n =>
                        n.IdUsuario == idUsuario &&
                        n.Estado
                )
                .OrderByDescending(
                    n => n.FechaCreacion
                )
                .Select(
                    n => new
                    {
                        // Id de la notificación.
                        idNotificacion =
                            n.IdNotificacion,

                        // Título.
                        titulo =
                            n.Titulo,

                        // Mensaje.
                        mensaje =
                            n.Mensaje,

                        // Tipo.
                        tipo =
                            n.Tipo,

                        // Fecha.
                        fechaCreacion =
                            n.FechaCreacion,

                        // Estado de lectura.
                        leida =
                            n.Leida
                    }
                )
                .ToListAsync();

        // Devuelve la lista.
        return Ok(
            notificaciones
        );
    }

    // Obtiene una notificación.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Notificacion>>
        GetNotificacion(int id)
    {
        // Busca la notificación.
        var notificacion =
            await _context.Notificacions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    n =>
                        n.IdNotificacion == id
                );

        // Valida que exista.
        if (notificacion == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Devuelve el registro.
        return Ok(
            notificacion
        );
    }

    // Crea una notificación.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPost]
    public async Task<ActionResult<Notificacion>>
        PostNotificacion(
            Notificacion notificacion)
    {
        // Revisa que exista el usuario.
        var usuarioExiste =
            await _context.Usuarios
                .AnyAsync(
                    u =>
                        u.IdUsuario ==
                        notificacion.IdUsuario
                );

        // Valida el usuario.
        if (!usuarioExiste)
        {
            return BadRequest(
                "El usuario no existe."
            );
        }

        // Permite generar el id.
        notificacion.IdNotificacion =
            0;

        // Guarda la fecha.
        notificacion.FechaCreacion =
            DateTime.Now;

        // Inicia como no leída.
        notificacion.Leida =
            false;

        // Mantiene la notificación activa.
        notificacion.Estado =
            true;

        // Agrega la notificación.
        _context.Notificacions.Add(
            notificacion
        );

        // Guarda los cambios.
        await _context
            .SaveChangesAsync();

        // Devuelve la notificación creada.
        return CreatedAtAction(
            nameof(GetNotificacion),
            new
            {
                id =
                    notificacion.IdNotificacion
            },
            notificacion
        );
    }

    // Actualiza una notificación.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        PutNotificacion(
            int id,
            Notificacion notificacion)
    {
        // Busca la notificación.
        var notificacionActual =
            await _context.Notificacions
                .FindAsync(id);

        // Valida que exista.
        if (notificacionActual == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Revisa el usuario.
        var usuarioExiste =
            await _context.Usuarios
                .AnyAsync(
                    u =>
                        u.IdUsuario ==
                        notificacion.IdUsuario
                );

        // Valida el usuario.
        if (!usuarioExiste)
        {
            return BadRequest(
                "El usuario no existe."
            );
        }

        // Actualiza el usuario.
        notificacionActual.IdUsuario =
            notificacion.IdUsuario;

        // Actualiza el título.
        notificacionActual.Titulo =
            notificacion.Titulo;

        // Actualiza el mensaje.
        notificacionActual.Mensaje =
            notificacion.Mensaje;

        // Actualiza el tipo.
        notificacionActual.Tipo =
            notificacion.Tipo;

        // Actualiza el estado de lectura.
        notificacionActual.Leida =
            notificacion.Leida;

        // Actualiza el estado.
        notificacionActual.Estado =
            notificacion.Estado;

        // Guarda los cambios.
        await _context
            .SaveChangesAsync();

        // Confirma la actualización.
        return NoContent();
    }

    // Marca una notificación como leída.
    [Authorize(Roles = "Cliente")]
    [HttpPut("{id:int}/marcar-leida")]
    public async Task<IActionResult>
        MarcarComoLeida(int id)
    {
        // Obtiene el usuario del token.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        // Valida el usuario.
        if (
            string.IsNullOrWhiteSpace(
                idUsuarioTexto
            ) ||
            !int.TryParse(
                idUsuarioTexto,
                out var idUsuario
            )
        )
        {
            return Unauthorized(
                "No se pudo identificar al usuario."
            );
        }

        // Busca una notificación propia.
        var notificacion =
            await _context.Notificacions
                .FirstOrDefaultAsync(
                    n =>
                        n.IdNotificacion == id &&
                        n.IdUsuario == idUsuario
                );

        // Valida que exista.
        if (notificacion == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Marca como leída.
        notificacion.Leida =
            true;

        // Guarda el cambio.
        await _context
            .SaveChangesAsync();

        // Confirma la actualización.
        return NoContent();
    }

    // Elimina una notificación.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteNotificacion(int id)
    {
        // Busca la notificación.
        var notificacion =
            await _context.Notificacions
                .FindAsync(id);

        // Valida que exista.
        if (notificacion == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Elimina el registro.
        _context.Notificacions.Remove(
            notificacion
        );

        // Guarda el cambio.
        await _context
            .SaveChangesAsync();

        // Confirma la eliminación.
        return NoContent();
    }
}