// Permite usar autorización.
using Microsoft.AspNetCore.Authorization;

// Permite crear endpoints API.
using Microsoft.AspNetCore.Mvc;

// Permite consultar la base de datos.
using Microsoft.EntityFrameworkCore;

// Permite leer datos del token.
using System.Security.Claims;

// Importa el contexto.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades.
using TiendaOnline.Dominio.Entidades;

// Importa los servicios.
using TiendaOnline.Dominio.InterfacesLN;

namespace TiendaOnline.API.Controllers;

// Requiere usuario autenticado.
[Authorize]

// Define un controlador API.
[ApiController]

// Ruta principal del controlador.
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    // Contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Servicio para generar PDF.
    private readonly IPdfServicio _pdfServicio;

    // Servicio para enviar correo.
    private readonly ICorreoServicio _correoServicio;

    // Permite registrar errores.
    private readonly ILogger<PagosController> _logger;

    // Recibe las dependencias.
    public PagosController(
        TiendaOnlineContext context,
        IPdfServicio pdfServicio,
        ICorreoServicio correoServicio,
        ILogger<PagosController> logger)
    {
        // Guarda el contexto.
        _context = context;

        // Guarda el servicio PDF.
        _pdfServicio = pdfServicio;

        // Guarda el servicio de correo.
        _correoServicio = correoServicio;

        // Guarda el logger.
        _logger = logger;
    }

    // Obtiene todos los pagos.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pago>>>
        GetPagos()
    {
        // Consulta los pagos.
        var pagos =
            await _context.Pagos
                .AsNoTracking()
                .OrderByDescending(
                    p => p.IdPago
                )
                .ToListAsync();

        // Devuelve los resultados.
        return Ok(pagos);
    }

    // Obtiene un pago.
    [HttpGet("{id:int}")]
    public async Task<IActionResult>
        GetPago(int id)
    {
        // Busca el pago.
        var pago =
            await _context.Pagos
                .AsNoTracking()
                .Where(
                    p => p.IdPago == id
                )
                .Select(
                    p => new
                    {
                        // Id del pago.
                        idPago =
                            p.IdPago,

                        // Pedido relacionado.
                        idPedido =
                            p.IdPedido,

                        // Usuario dueño del pedido.
                        idUsuario =
                            p.IdPedidoNavigation
                                .IdUsuario,

                        // Método usado.
                        metodoPago =
                            p.MetodoPago,

                        // Referencia generada.
                        referencia =
                            p.Referencia,

                        // Monto pagado.
                        monto =
                            p.Monto,

                        // Fecha del pago.
                        fechaPago =
                            p.FechaPago,

                        // Estado del pago.
                        estado =
                            p.Estado,

                        // Id del método.
                        idMetodoPago =
                            p.IdMetodoPago,

                        // Id del estado.
                        idEstadoPago =
                            p.IdEstadoPago
                    }
                )
                .FirstOrDefaultAsync();

        // Valida que exista.
        if (pago == null)
        {
            return NotFound(
                "El pago no existe."
            );
        }

        // Obtiene el rol.
        var rol =
            User.FindFirstValue(
                ClaimTypes.Role
            );

        // Admin y empleado pueden verlo.
        if (
            rol == "Administrador" ||
            rol == "Empleado"
        )
        {
            return Ok(pago);
        }

        // Obtiene el usuario del token.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        // Valida el usuario.
        if (
            !int.TryParse(
                idUsuarioTexto,
                out int idUsuario
            )
        )
        {
            return Unauthorized(
                "No se pudo identificar al usuario."
            );
        }

        // Evita ver pagos ajenos.
        if (
            pago.idUsuario != idUsuario
        )
        {
            return Forbid();
        }

        // Devuelve el pago.
        return Ok(pago);
    }

    // Realiza el pago.
    [Authorize(Roles = "Cliente")]
    [HttpPost("pagar")]
    public async Task<IActionResult>
        PagarPedido(
            [FromBody] PagarPedidoDto dto)
    {
        // Obtiene el usuario del token.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        // Valida el usuario.
        if (
            !int.TryParse(
                idUsuarioTexto,
                out int idUsuario
            )
        )
        {
            return Unauthorized(
                "No se pudo identificar al usuario."
            );
        }

        // Valida el pedido.
        if (dto.IdPedido <= 0)
        {
            return BadRequest(
                "Debe indicar un pedido válido."
            );
        }

        // Valida el método.
        if (dto.IdMetodoPago <= 0)
        {
            return BadRequest(
                "Debe seleccionar un método de pago."
            );
        }

        // Busca el pedido.
        var pedido =
            await _context.Pedidos
                .FirstOrDefaultAsync(
                    p =>
                        p.IdPedido ==
                        dto.IdPedido
                );

        // Valida que exista.
        if (pedido == null)
        {
            return NotFound(
                "El pedido no existe."
            );
        }

        // Valida el propietario.
        if (
            pedido.IdUsuario !=
            idUsuario
        )
        {
            return Forbid();
        }

        // Evita pagar cancelados.
        if (
            pedido.Estado ==
            "Cancelado"
        )
        {
            return BadRequest(
                "No se puede pagar un pedido cancelado."
            );
        }

        // Revisa si ya fue pagado.
        var yaEstaPagado =
            await _context.Pagos
                .AnyAsync(
                    p =>
                        p.IdPedido ==
                            pedido.IdPedido &&
                        p.Estado ==
                            "Aprobado"
                );

        // Evita pagos duplicados.
        if (yaEstaPagado)
        {
            return BadRequest(
                "Este pedido ya se encuentra pagado."
            );
        }

        // Busca el método de pago.
        var metodoPago =
            await _context.MetodoPagos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    m =>
                        m.IdMetodoPago ==
                            dto.IdMetodoPago &&
                        m.Estado
                );

        // Valida el método.
        if (metodoPago == null)
        {
            return BadRequest(
                "El método de pago seleccionado no existe o está inactivo."
            );
        }

        // Busca el estado Aprobado.
        var estadoPagoAprobado =
            await _context.EstadoPagos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e =>
                        e.Nombre ==
                            "Aprobado" &&
                        e.Estado
                );

        // Valida el estado.
        if (estadoPagoAprobado == null)
        {
            return BadRequest(
                "No se encontró el estado de pago Aprobado."
            );
        }

        // Busca el estado Pagado.
        var estadoPedidoPagado =
            await _context.EstadoPedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e =>
                        e.Nombre ==
                            "Pagado" &&
                        e.Estado
                );

        // Valida el estado.
        if (estadoPedidoPagado == null)
        {
            return BadRequest(
                "No se encontró el estado de pedido Pagado."
            );
        }

        // Crea el pago.
        var nuevoPago =
            new Pago
            {
                // Relaciona el pedido.
                IdPedido =
                    pedido.IdPedido,

                // Guarda el método.
                MetodoPago =
                    metodoPago.Nombre,

                // Genera la referencia.
                Referencia =
                    $"PAGO-{pedido.IdPedido}-{DateTime.UtcNow:yyyyMMddHHmmss}",

                // Usa el total real.
                Monto =
                    pedido.Total,

                // Guarda la fecha.
                FechaPago =
                    DateTime.UtcNow,

                // Marca como aprobado.
                Estado =
                    estadoPagoAprobado.Nombre,

                // Relaciona el método.
                IdMetodoPago =
                    metodoPago.IdMetodoPago,

                // Relaciona el estado.
                IdEstadoPago =
                    estadoPagoAprobado
                        .IdEstadoPago
            };

        // Agrega el pago.
        _context.Pagos.Add(
            nuevoPago
        );

        // Cambia el estado del pedido.
        pedido.IdEstadoPedido =
            estadoPedidoPagado
                .IdEstadoPedido;

        // Guarda el nombre Pagado.
        pedido.Estado =
            estadoPedidoPagado
                .Nombre;

        // Guarda primero el pago.
        await _context
            .SaveChangesAsync();

        // Crea una notificación del pago.
        var notificacionPago =
            new Notificacion
            {
                // Usuario que recibirá la notificación.
                IdUsuario =
                    pedido.IdUsuario,

                // Título mostrado en la pantalla.
                Titulo =
                    "Pago aprobado",

                // Mensaje para el cliente.
                Mensaje =
                    $"El pago del pedido #{pedido.IdPedido} fue aprobado correctamente.",

                // Tipo de notificación.
                Tipo =
                    "Pago",

                // Guarda la fecha actual.
                FechaCreacion =
                    DateTime.UtcNow,

                // Inicia como no leída.
                Leida =
                    false,

                // Mantiene la notificación activa.
                Estado =
                    true
            };

        // Agrega la notificación.
        _context.Notificacions.Add(
            notificacionPago
        );

        // Guarda la notificación.
        await _context
            .SaveChangesAsync();

        // Indica si se envió el correo.
        var correoEnviado =
            false;

        // Mensaje para Angular.
        var mensajeCorreo =
            "Comprobante enviado al correo.";

        try
        {
            // Busca los datos para el PDF.
            var pedidoComprobante =
                await _context.Pedidos
                    .AsNoTracking()

                    // Incluye el cliente.
                    .Include(
                        p =>
                            p.IdUsuarioNavigation
                    )

                    // Incluye los productos.
                    .Include(
                        p =>
                            p.DetallePedidos
                    )
                    .ThenInclude(
                        d =>
                            d.IdProductoNavigation
                    )

                    // Busca el pedido pagado.
                    .FirstOrDefaultAsync(
                        p =>
                            p.IdPedido ==
                            pedido.IdPedido
                    );

            // Valida los datos.
            if (
                pedidoComprobante != null
            )
            {
                // Genera el PDF.
                var pdf =
                    _pdfServicio
                        .GenerarComprobante(
                            pedidoComprobante,
                            nuevoPago
                        );

                // Obtiene el cliente.
                var usuario =
                    pedidoComprobante
                        .IdUsuarioNavigation;

                // Forma el nombre completo.
                var nombreCliente =
                    $"{usuario.Nombre} {usuario.Apellido}";

                // Envía el correo.
                await _correoServicio
                    .EnviarComprobanteAsync(
                        usuario.Correo,
                        nombreCliente,
                        pedido.IdPedido,
                        pdf
                    );

                // Confirma el envío.
                correoEnviado =
                    true;
            }
        }
        catch (Exception ex)
        {
            // Registra el error del correo.
            _logger.LogError(
                ex,
                "No se pudo enviar el comprobante del pedido {IdPedido}.",
                pedido.IdPedido
            );

            // El pago sigue aprobado.
            mensajeCorreo =
                "El pago fue aprobado, pero el comprobante no pudo enviarse.";
        }

        // Devuelve el resultado.
        return Ok(
            new
            {
                // Mensaje principal.
                mensaje =
                    "Pago realizado correctamente.",

                // Pedido pagado.
                idPedido =
                    pedido.IdPedido,

                // Pago generado.
                idPago =
                    nuevoPago.IdPago,

                // Estado del pago.
                estadoPago =
                    "Pagado",

                // Estado del pedido.
                estadoPedido =
                    pedido.Estado,

                // Método utilizado.
                metodoPago =
                    nuevoPago.MetodoPago,

                // Monto pagado.
                monto =
                    nuevoPago.Monto,

                // Referencia.
                referencia =
                    nuevoPago.Referencia,

                // Fecha.
                fechaPago =
                    nuevoPago.FechaPago,

                // Resultado del correo.
                correoEnviado,

                // Mensaje del correo.
                mensajeCorreo
            }
        );
    }

    // Modifica un pago.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        PutPago(
            int id,
            Pago pago)
    {
        // Busca el pago.
        var pagoActual =
            await _context.Pagos
                .FindAsync(id);

        // Valida que exista.
        if (pagoActual == null)
        {
            return NotFound(
                "El pago no existe."
            );
        }

        // Actualiza el pedido.
        pagoActual.IdPedido =
            pago.IdPedido;

        // Actualiza el método.
        pagoActual.MetodoPago =
            pago.MetodoPago;

        // Actualiza la referencia.
        pagoActual.Referencia =
            pago.Referencia;

        // Actualiza el monto.
        pagoActual.Monto =
            pago.Monto;

        // Actualiza la fecha.
        pagoActual.FechaPago =
            pago.FechaPago;

        // Actualiza el estado.
        pagoActual.Estado =
            pago.Estado;

        // Actualiza el método relacionado.
        pagoActual.IdMetodoPago =
            pago.IdMetodoPago;

        // Actualiza el estado relacionado.
        pagoActual.IdEstadoPago =
            pago.IdEstadoPago;

        // Guarda los cambios.
        await _context
            .SaveChangesAsync();

        // Devuelve respuesta vacía.
        return NoContent();
    }

    // Elimina un pago.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeletePago(int id)
    {
        // Busca el pago.
        var pago =
            await _context.Pagos
                .FindAsync(id);

        // Valida que exista.
        if (pago == null)
        {
            return NotFound(
                "El pago no existe."
            );
        }

        // Elimina el pago.
        _context.Pagos.Remove(
            pago
        );

        // Guarda el cambio.
        await _context
            .SaveChangesAsync();

        // Devuelve respuesta vacía.
        return NoContent();
    }
}

// Datos necesarios para pagar.
public class PagarPedidoDto
{
    // Pedido seleccionado.
    public int IdPedido { get; set; }

    // Método seleccionado.
    public int IdMetodoPago { get; set; }
}