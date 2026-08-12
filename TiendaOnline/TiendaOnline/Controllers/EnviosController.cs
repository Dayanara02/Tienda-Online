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

// Define la ruta principal api/Envios.
[Route("api/[controller]")]
public class EnviosController : ControllerBase
{
    // Guarda el contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto mediante inyección de dependencias.
    public EnviosController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/Envios
    // Permite a Administrador y Empleado consultar todos los envíos.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Envio>>>
        GetEnvios()
    {
        // Consulta todos los envíos ordenados por fecha.
        return await _context.Envios
            .AsNoTracking()
            .OrderByDescending(
                envio => envio.FechaEnvio
            )
            .ToListAsync();
    }

    // GET: api/Envios/5
    // Permite consultar un envío específico por identificador.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Envio>>
        GetEnvio(int id)
    {
        // Busca el envío solicitado.
        var envio =
            await _context.Envios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e => e.IdEnvio == id
                );

        // Devuelve 404 si no existe.
        if (envio == null)
        {
            return NotFound(
                "El envío no existe."
            );
        }

        // Devuelve el envío encontrado.
        return Ok(envio);
    }

    // GET: api/Envios/pedido/5
    // Permite al Cliente consultar el seguimiento de su propio pedido.
    [Authorize(Roles = "Cliente")]
    [HttpGet("pedido/{idPedido:int}")]
    public async Task<IActionResult>
        GetEnvioPorPedido(int idPedido)
    {
        // Obtiene el identificador del usuario desde el token JWT.
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

        // Busca el pedido y comprueba que pertenezca al cliente.
        var pedido =
            await _context.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p =>
                        p.IdPedido == idPedido &&
                        p.IdUsuario == idUsuario
                );

        // Evita consultar pedidos de otro usuario.
        if (pedido == null)
        {
            return NotFound(
                "El pedido no existe o no pertenece al usuario."
            );
        }

        // Busca el envío relacionado con el pedido.
        var envio =
            await _context.Envios
                .AsNoTracking()
                .Where(
                    e =>
                        e.IdPedido == idPedido
                )
                .Select(
                    e => new
                    {
                        // Identificador del envío.
                        idEnvio =
                            e.IdEnvio,

                        // Identificador del pedido.
                        idPedido =
                            e.IdPedido,

                        // Empresa encargada del envío.
                        empresaEnvio =
                            e.EmpresaEnvio,

                        // Número utilizado para seguimiento.
                        numeroSeguimiento =
                            e.NumeroSeguimiento,

                        // Fecha en que salió el pedido.
                        fechaEnvio =
                            e.FechaEnvio,

                        // Fecha en que fue entregado.
                        fechaEntrega =
                            e.FechaEntrega,

                        // Estado actual del envío.
                        estado =
                            e.Estado,

                        // Identificador de la dirección.
                        idDireccion =
                            e.IdDireccion,

                        // Obtiene la dirección exacta relacionada.
                        direccion =
                            e.IdDireccionNavigation != null
                                ? e.IdDireccionNavigation.DireccionExacta
                                : null,

                        // Obtiene la provincia relacionada.
                        provincia =
                            e.IdDireccionNavigation != null
                                ? e.IdDireccionNavigation.Provincia
                                : null,

                        // Obtiene el cantón relacionado.
                        canton =
                            e.IdDireccionNavigation != null
                                ? e.IdDireccionNavigation.Canton
                                : null,

                        // Obtiene el distrito relacionado.
                        distrito =
                            e.IdDireccionNavigation != null
                                ? e.IdDireccionNavigation.Distrito
                                : null
                    }
                )
                .FirstOrDefaultAsync();

        // Informa si todavía no existe un envío.
        if (envio == null)
        {
            return NotFound(
                "Este pedido todavía no tiene un envío registrado."
            );
        }

        // Devuelve la información necesaria para seguimiento.
        return Ok(envio);
    }

    // POST: api/Envios
    // Permite crear envíos desde Administración o Empleado.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPost]
    public async Task<ActionResult<Envio>>
        PostEnvio(Envio envio)
    {
        // Comprueba que el pedido exista.
        var pedidoExiste =
            await _context.Pedidos
                .AnyAsync(
                    p =>
                        p.IdPedido ==
                        envio.IdPedido
                );

        // Informa si el pedido no existe.
        if (!pedidoExiste)
        {
            return BadRequest(
                "El pedido no existe."
            );
        }

        // Comprueba que la dirección exista.
        var direccionExiste =
            await _context.DireccionUsuarios
                .AnyAsync(
                    d =>
                        d.IdDireccion ==
                        envio.IdDireccion
                );

        // Informa si la dirección no existe.
        if (!direccionExiste)
        {
            return BadRequest(
                "La dirección no existe."
            );
        }

        // Evita registrar dos envíos para el mismo pedido.
        var pedidoYaTieneEnvio =
            await _context.Envios
                .AnyAsync(
                    e =>
                        e.IdPedido ==
                        envio.IdPedido
                );

        // Informa si ya existe un envío.
        if (pedidoYaTieneEnvio)
        {
            return Conflict(
                "El pedido ya tiene un envío registrado."
            );
        }

        // Permite que SQL Server genere el identificador.
        envio.IdEnvio =
            0;

        // Asigna Pendiente si no se envía ningún estado.
        if (
            string.IsNullOrWhiteSpace(
                envio.Estado
            )
        )
        {
            envio.Estado =
                "Pendiente";
        }

        // Agrega el nuevo envío.
        _context.Envios.Add(
            envio
        );

        // Guarda los cambios.
        await _context.SaveChangesAsync();

        // Devuelve el envío creado.
        return CreatedAtAction(
            nameof(GetEnvio),
            new
            {
                id = envio.IdEnvio
            },
            envio
        );
    }

    // PUT: api/Envios/5
    // Permite actualizar los datos del envío.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        PutEnvio(
            int id,
            Envio envio)
    {
        // Busca el envío existente.
        var envioActual =
            await _context.Envios
                .FindAsync(id);

        // Devuelve 404 si no existe.
        if (envioActual == null)
        {
            return NotFound(
                "El envío no existe."
            );
        }

        // Comprueba que el pedido exista.
        var pedidoExiste =
            await _context.Pedidos
                .AnyAsync(
                    p =>
                        p.IdPedido ==
                        envio.IdPedido
                );

        // Informa si el pedido no existe.
        if (!pedidoExiste)
        {
            return BadRequest(
                "El pedido no existe."
            );
        }

        // Comprueba que la dirección exista.
        var direccionExiste =
            await _context.DireccionUsuarios
                .AnyAsync(
                    d =>
                        d.IdDireccion ==
                        envio.IdDireccion
                );

        // Informa si la dirección no existe.
        if (!direccionExiste)
        {
            return BadRequest(
                "La dirección no existe."
            );
        }

        // Evita relacionar el pedido con otro envío.
        var otroEnvioDelPedido =
            await _context.Envios
                .AnyAsync(
                    e =>
                        e.IdPedido ==
                            envio.IdPedido &&
                        e.IdEnvio != id
                );

        // Informa si el pedido ya tiene otro envío.
        if (otroEnvioDelPedido)
        {
            return Conflict(
                "El pedido ya tiene otro envío registrado."
            );
        }

        // Actualiza el pedido relacionado.
        envioActual.IdPedido =
            envio.IdPedido;

        // Actualiza la dirección relacionada.
        envioActual.IdDireccion =
            envio.IdDireccion;

        // Actualiza la empresa encargada.
        envioActual.EmpresaEnvio =
            envio.EmpresaEnvio;

        // Actualiza el número de seguimiento.
        envioActual.NumeroSeguimiento =
            envio.NumeroSeguimiento;

        // Actualiza la fecha de envío.
        envioActual.FechaEnvio =
            envio.FechaEnvio;

        // Actualiza la fecha de entrega.
        envioActual.FechaEntrega =
            envio.FechaEntrega;

        // Actualiza el estado actual.
        envioActual.Estado =
            envio.Estado;

        // Guarda los cambios realizados.
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Envios/5
    // Permite eliminar un envío solamente al Administrador.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteEnvio(int id)
    {
        // Busca el envío solicitado.
        var envio =
            await _context.Envios
                .FindAsync(id);

        // Devuelve 404 si no existe.
        if (envio == null)
        {
            return NotFound(
                "El envío no existe."
            );
        }

        // Elimina el registro.
        _context.Envios.Remove(
            envio
        );

        // Guarda los cambios.
        await _context.SaveChangesAsync();

        return NoContent();
    }
}