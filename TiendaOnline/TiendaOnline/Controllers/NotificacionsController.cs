// Importa las herramientas necesarias para crear controladores API.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para consultar la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa autorización para proteger los endpoints.
using Microsoft.AspNetCore.Authorization;

// Importa las entidades del dominio.
using TiendaOnline.Dominio.Entidades;

// Permite leer el identificador del usuario desde el token JWT.
using System.Security.Claims;

namespace TiendaOnline.API.Controllers;

// Obliga a que el usuario esté autenticado.
[Authorize]

// Indica que esta clase funciona como controlador API.
[ApiController]

// Define la ruta principal api/Notificacions.
[Route("api/[controller]")]
public class NotificacionsController : ControllerBase
{
    // Guarda el contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto mediante inyección de dependencias.
    public NotificacionsController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Notificacions
    // Permite a Administrador y Empleado consultar todas las notificaciones.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notificacion>>>
        GetNotificacions()
    {
        // Consulta todas las notificaciones.
        return await _context.Notificacions
            .AsNoTracking()
            .OrderByDescending(
                n => n.FechaCreacion
            )
            .ToListAsync();
    }

    // GET: api/Notificacions/mis-notificaciones
    // Permite al Cliente consultar solamente sus propias notificaciones.
    [Authorize(Roles = "Cliente")]
    [HttpGet("mis-notificaciones")]
    public async Task<IActionResult>
        GetMisNotificaciones()
    {
        // Obtiene el identificador del usuario desde el token.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        // Comprueba que el token contenga un usuario válido.
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

        // Consulta solamente las notificaciones del usuario conectado.
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
                        // Identificador de la notificación.
                        idNotificacion =
                            n.IdNotificacion,

                        // Título mostrado al cliente.
                        titulo =
                            n.Titulo,

                        // Mensaje de la notificación.
                        mensaje =
                            n.Mensaje,

                        // Tipo de notificación.
                        tipo =
                            n.Tipo,

                        // Fecha en que fue creada.
                        fechaCreacion =
                            n.FechaCreacion,

                        // Indica si ya fue leída.
                        leida =
                            n.Leida
                    }
                )
                .ToListAsync();

        // Devuelve la lista del cliente.
        return Ok(notificaciones);
    }

    // GET: api/Notificacions/5
    // Permite consultar una notificación específica.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Notificacion>>
        GetNotificacion(int id)
    {
        // Busca la notificación por identificador.
        var notificacion =
            await _context.Notificacions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    n =>
                        n.IdNotificacion == id
                );

        // Devuelve 404 si no existe.
        if (notificacion == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Devuelve la notificación encontrada.
        return Ok(notificacion);
    }

    // POST: api/Notificacions
    // Permite crear notificaciones desde Administración o Empleado.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPost]
    public async Task<ActionResult<Notificacion>>
        PostNotificacion(
            Notificacion notificacion)
    {
        // Comprueba que el usuario exista.
        var usuarioExiste =
            await _context.Usuarios
                .AnyAsync(
                    u =>
                        u.IdUsuario ==
                        notificacion.IdUsuario
                );

        if (!usuarioExiste)
        {
            return BadRequest(
                "El usuario no existe."
            );
        }

        // Permite que SQL Server genere el identificador.
        notificacion.IdNotificacion =
            0;

        // Guarda automáticamente la fecha actual.
        notificacion.FechaCreacion =
            DateTime.Now;

        // Una notificación nueva inicia sin leer.
        notificacion.Leida =
            false;

        // Activa la notificación si no fue marcada.
        notificacion.Estado =
            true;

        // Agrega la notificación.
        _context.Notificacions.Add(
            notificacion
        );

        // Guarda los cambios.
        await _context.SaveChangesAsync();

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

    // PUT: api/Notificacions/5
    // Permite actualizar una notificación.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        PutNotificacion(
            int id,
            Notificacion notificacion)
    {
        // Busca la notificación existente.
        var notificacionActual =
            await _context.Notificacions
                .FindAsync(id);

        // Devuelve 404 si no existe.
        if (notificacionActual == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Comprueba que el usuario exista.
        var usuarioExiste =
            await _context.Usuarios
                .AnyAsync(
                    u =>
                        u.IdUsuario ==
                        notificacion.IdUsuario
                );

        if (!usuarioExiste)
        {
            return BadRequest(
                "El usuario no existe."
            );
        }

        // Actualiza los datos permitidos.
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

        // Guarda los cambios.
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/Notificacions/5/marcar-leida
    // Permite al Cliente marcar una notificación propia como leída.
    [Authorize(Roles = "Cliente")]
    [HttpPut("{id:int}/marcar-leida")]
    public async Task<IActionResult>
        MarcarComoLeida(int id)
    {
        // Obtiene el usuario desde el token JWT.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        // Comprueba que el usuario sea válido.
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

        // Busca solamente una notificación del usuario conectado.
        var notificacion =
            await _context.Notificacions
                .FirstOrDefaultAsync(
                    n =>
                        n.IdNotificacion == id &&
                        n.IdUsuario == idUsuario
                );

        // Evita modificar notificaciones de otro usuario.
        if (notificacion == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Marca la notificación como leída.
        notificacion.Leida =
            true;

        // Guarda el cambio.
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Notificacions/5
    // Permite eliminar notificaciones solamente al Administrador.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteNotificacion(int id)
    {
        // Busca la notificación.
        var notificacion =
            await _context.Notificacions
                .FindAsync(id);

        // Devuelve 404 si no existe.
        if (notificacion == null)
        {
            return NotFound(
                "La notificación no existe."
            );
        }

        // Elimina la notificación.
        _context.Notificacions.Remove(
            notificacion
        );

        // Guarda los cambios.
        await _context.SaveChangesAsync();

        return NoContent();
    }
}