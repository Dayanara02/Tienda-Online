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

// Requiere que el usuario esté autenticado.
[Authorize]

// Indica que esta clase es un controlador API.
[ApiController]

// Define la ruta principal.
[Route("api/[controller]")]
public class EnviosController : ControllerBase
{
    // Guarda el contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto.
    public EnviosController(
        TiendaOnlineContext context)
    {
        // Guarda el contexto recibido.
        _context = context;
    }

    // Obtiene todos los envíos.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Envio>>>
        GetEnvios()
    {
        // Consulta los envíos.
        return await _context.Envios
            .AsNoTracking()
            .OrderByDescending(
                envio => envio.FechaEnvio
            )
            .ToListAsync();
    }

    // Obtiene un envío por id.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Envio>>
        GetEnvio(int id)
    {
        // Busca el envío.
        var envio =
            await _context.Envios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e => e.IdEnvio == id
                );

        // Valida que exista.
        if (envio == null)
        {
            return NotFound(
                "El envío no existe."
            );
        }

        // Devuelve el envío.
        return Ok(envio);
    }

    // Obtiene el envío de un pedido.
    [Authorize(Roles = "Cliente")]
    [HttpGet("pedido/{idPedido:int}")]
    public async Task<IActionResult>
        GetEnvioPorPedido(int idPedido)
    {
        // Obtiene el id del usuario.
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

        // Busca el pedido del cliente.
        var pedido =
            await _context.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p =>
                        p.IdPedido == idPedido &&
                        p.IdUsuario == idUsuario
                );

        // Evita consultar pedidos ajenos.
        if (pedido == null)
        {
            return NotFound(
                "El pedido no existe o no pertenece al usuario."
            );
        }

        // Busca el envío relacionado.
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
                        // Id del envío.
                        idEnvio =
                            e.IdEnvio,

                        // Id del pedido.
                        idPedido =
                            e.IdPedido,

                        // Empresa de envío.
                        empresaEnvio =
                            e.EmpresaEnvio,

                        // Número de seguimiento.
                        numeroSeguimiento =
                            e.NumeroSeguimiento,

                        // Fecha de envío.
                        fechaEnvio =
                            e.FechaEnvio,

                        // Fecha de entrega.
                        fechaEntrega =
                            e.FechaEntrega,

                        // Estado actual.
                        estado =
                            e.Estado,

                        // Id de la dirección.
                        idDireccion =
                            e.IdDireccion,

                        // Dirección guardada en el pedido.
                        direccion =
                            e.IdPedidoNavigation
                                .DireccionEntrega,

                        // Provincia.
                        provincia =
                            e.IdDireccionNavigation
                                .Provincia,

                        // Cantón.
                        canton =
                            e.IdDireccionNavigation
                                .Canton,

                        // Distrito.
                        distrito =
                            e.IdDireccionNavigation
                                .Distrito
                    }
                )
                .FirstOrDefaultAsync();

        // Valida que exista el envío.
        if (envio == null)
        {
            return NotFound(
                "Este pedido todavía no tiene un envío registrado."
            );
        }

        // Devuelve el seguimiento.
        return Ok(envio);
    }

    // Crea un envío.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPost]
    public async Task<ActionResult<Envio>>
        PostEnvio(Envio envio)
    {
        // Busca el pedido.
        var pedido =
            await _context.Pedidos
                .FirstOrDefaultAsync(
                    p =>
                        p.IdPedido ==
                        envio.IdPedido
                );

        // Valida el pedido.
        if (pedido == null)
        {
            return BadRequest(
                "El pedido no existe."
            );
        }

        // Revisa si existe la dirección.
        var direccionExiste =
            await _context.DireccionUsuarios
                .AnyAsync(
                    d =>
                        d.IdDireccion ==
                        envio.IdDireccion
                );

        // Valida la dirección.
        if (!direccionExiste)
        {
            return BadRequest(
                "La dirección no existe."
            );
        }

        // Revisa si el pedido ya tiene envío.
        var pedidoYaTieneEnvio =
            await _context.Envios
                .AnyAsync(
                    e =>
                        e.IdPedido ==
                        envio.IdPedido
                );

        // Evita envíos duplicados.
        if (pedidoYaTieneEnvio)
        {
            return Conflict(
                "El pedido ya tiene un envío registrado."
            );
        }

        // Permite que SQL genere el id.
        envio.IdEnvio =
            0;

        // Asigna estado inicial.
        if (
            string.IsNullOrWhiteSpace(
                envio.Estado
            )
        )
        {
            envio.Estado =
                "Pendiente";
        }

        // Agrega el envío.
        _context.Envios.Add(
            envio
        );

        // Guarda los cambios.
        await _context
            .SaveChangesAsync();

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

    // Actualiza un envío.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        PutEnvio(
            int id,
            Envio envio)
    {
        // Busca el envío actual.
        var envioActual =
            await _context.Envios
                .FindAsync(id);

        // Valida que exista.
        if (envioActual == null)
        {
            return NotFound(
                "El envío no existe."
            );
        }

        // Guarda el estado anterior.
        var estadoAnterior =
            envioActual.Estado;

        // Busca el pedido relacionado.
        var pedido =
            await _context.Pedidos
                .FirstOrDefaultAsync(
                    p =>
                        p.IdPedido ==
                        envio.IdPedido
                );

        // Valida el pedido.
        if (pedido == null)
        {
            return BadRequest(
                "El pedido no existe."
            );
        }

        // Revisa la dirección.
        var direccionExiste =
            await _context.DireccionUsuarios
                .AnyAsync(
                    d =>
                        d.IdDireccion ==
                        envio.IdDireccion
                );

        // Valida la dirección.
        if (!direccionExiste)
        {
            return BadRequest(
                "La dirección no existe."
            );
        }

        // Revisa si existe otro envío.
        var otroEnvioDelPedido =
            await _context.Envios
                .AnyAsync(
                    e =>
                        e.IdPedido ==
                            envio.IdPedido &&
                        e.IdEnvio != id
                );

        // Evita duplicados.
        if (otroEnvioDelPedido)
        {
            return Conflict(
                "El pedido ya tiene otro envío registrado."
            );
        }

        // Actualiza el pedido relacionado.
        envioActual.IdPedido =
            envio.IdPedido;

        // Actualiza la dirección.
        envioActual.IdDireccion =
            envio.IdDireccion;

        // Actualiza la empresa.
        envioActual.EmpresaEnvio =
            envio.EmpresaEnvio;

        // Actualiza el seguimiento.
        envioActual.NumeroSeguimiento =
            envio.NumeroSeguimiento;

        // Actualiza la fecha de envío.
        envioActual.FechaEnvio =
            envio.FechaEnvio;

        // Actualiza la fecha de entrega.
        envioActual.FechaEntrega =
            envio.FechaEntrega;

        // Actualiza el estado.
        envioActual.Estado =
            envio.Estado;

        // Comprueba si cambió a Enviado.
        var cambioAEnviado =
            envio.Estado.Equals(
                "Enviado",
                StringComparison.OrdinalIgnoreCase
            )
            &&
            !estadoAnterior.Equals(
                "Enviado",
                StringComparison.OrdinalIgnoreCase
            );

        // Comprueba si cambió a Entregado.
        var cambioAEntregado =
            envio.Estado.Equals(
                "Entregado",
                StringComparison.OrdinalIgnoreCase
            )
            &&
            !estadoAnterior.Equals(
                "Entregado",
                StringComparison.OrdinalIgnoreCase
            );

        // Si cambió a Enviado.
        if (cambioAEnviado)
        {
            // Actualiza el pedido.
            pedido.Estado =
                "Enviado";

            // Muestra la prueba en consola.
            Console.WriteLine(
                $"Creando notificación para usuario {pedido.IdUsuario}, pedido {pedido.IdPedido}"
            );

            // Crea la notificación.
            var notificacionEnviado =
                new Notificacion
                {
                    // Usuario que recibe.
                    IdUsuario =
                        pedido.IdUsuario,

                    // Título.
                    Titulo =
                        "Pedido enviado",

                    // Mensaje.
                    Mensaje =
                        $"Tu pedido #{pedido.IdPedido} ya fue enviado.",

                    // Tipo.
                    Tipo =
                        "Envio",

                    // Fecha actual.
                    FechaCreacion =
                        DateTime.UtcNow,

                    // Inicia como no leída.
                    Leida =
                        false,

                    // Mantiene el registro activo.
                    Estado =
                        true
                };

            // Agrega la notificación.
            _context.Notificacions.Add(
                notificacionEnviado
            );
        }

        // Si cambió a Entregado.
        if (cambioAEntregado)
        {
            // Actualiza el pedido.
            pedido.Estado =
                "Entregado";

            // Muestra la prueba en consola.
            Console.WriteLine(
                $"Creando notificación de entrega para usuario {pedido.IdUsuario}, pedido {pedido.IdPedido}"
            );

            // Crea la notificación.
            var notificacionEntregado =
                new Notificacion
                {
                    // Usuario que recibe.
                    IdUsuario =
                        pedido.IdUsuario,

                    // Título.
                    Titulo =
                        "Pedido entregado",

                    // Mensaje.
                    Mensaje =
                        $"Tu pedido #{pedido.IdPedido} fue entregado correctamente.",

                    // Tipo.
                    Tipo =
                        "Envio",

                    // Fecha actual.
                    FechaCreacion =
                        DateTime.UtcNow,

                    // Inicia como no leída.
                    Leida =
                        false,

                    // Mantiene el registro activo.
                    Estado =
                        true
                };

            // Agrega la notificación.
            _context.Notificacions.Add(
                notificacionEntregado
            );
        }

        // Guarda todos los cambios.
        await _context
            .SaveChangesAsync();

        // Confirma la actualización.
        return NoContent();
    }

    // Elimina un envío.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteEnvio(int id)
    {
        // Busca el envío.
        var envio =
            await _context.Envios
                .FindAsync(id);

        // Valida que exista.
        if (envio == null)
        {
            return NotFound(
                "El envío no existe."
            );
        }

        // Elimina el envío.
        _context.Envios.Remove(
            envio
        );

        // Guarda el cambio.
        await _context
            .SaveChangesAsync();

        // Confirma la eliminación.
        return NoContent();
    }
}