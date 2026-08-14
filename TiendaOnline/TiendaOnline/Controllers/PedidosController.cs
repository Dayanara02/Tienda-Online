// Permite utilizar atributos de autorización como [Authorize].
using Microsoft.AspNetCore.Authorization;

// Permite crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Permite trabajar con Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Permite obtener información del usuario desde el token JWT.
using System.Security.Claims;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa los DTO utilizados para los pedidos.
using TiendaOnline.Dominio.DTO;

// Importa las entidades de la base de datos.
using TiendaOnline.Dominio.Entidades;

// Importa la interfaz del servicio de pedidos.
using TiendaOnline.Dominio.InterfacesLN;

namespace TiendaOnline.API.Controllers
{
    // Define la ruta base: api/Pedidos.
    [Route("api/[controller]")]

    // Indica que es un controlador de API.
    [ApiController]

    // Requiere que el usuario esté autenticado.
    [Authorize]
    public class PedidosController : ControllerBase
    {
        // Contexto utilizado para acceder a la base de datos.
        private readonly TiendaOnlineContext _context;

        // Servicio encargado de crear pedidos.
        private readonly IPedidoServicio _pedidoServicio;

        // Constructor del controlador.
        public PedidosController(
            TiendaOnlineContext context,
            IPedidoServicio pedidoServicio)
        {
            // Guarda el contexto recibido.
            _context = context;

            // Guarda el servicio recibido.
            _pedidoServicio = pedidoServicio;
        }

        // Obtiene todos los pedidos registrados.
        // Solo Administradores y Empleados tienen acceso.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            // Consulta los pedidos y los ordena del más reciente al más antiguo.
            var pedidos = await _context.Pedidos
                .AsNoTracking()
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return Ok(pedidos);
        }

        // Obtiene solamente los pedidos del cliente autenticado.
        [Authorize(Roles = "Cliente")]
        [HttpGet("mis-pedidos")]
        public async Task<IActionResult> GetMisPedidos()
        {
            // Obtiene el ID del usuario desde el token JWT.
            var idUsuarioTexto =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Valida que el ID sea numérico.
            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario del token.");
            }

            // Busca únicamente los pedidos pertenecientes al usuario.
            var pedidos = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.IdUsuario == idUsuario)
                .OrderByDescending(p => p.FechaPedido)
                .Select(p => new
                {
                    // Información básica del pedido.
                    idPedido = p.IdPedido,
                    fechaPedido = p.FechaPedido,
                    estado = p.Estado,
                    subtotal = p.Subtotal,
                    impuesto = p.Impuesto,
                    descuento = p.Descuento,
                    total = p.Total,
                    direccionEntrega = p.DireccionEntrega,

                    // Determina si el pedido está pagado, pendiente o cancelado.
                    estadoPago =
                        p.Estado == "Cancelado"
                            ? "Cancelado"
                            : p.Pagos
                                .OrderByDescending(pago => pago.IdPago)
                                .Select(pago => pago.Estado)
                                .FirstOrDefault() == "Aprobado"
                                    ? "Pagado"
                                    : "Pendiente"
                })
                .ToListAsync();

            return Ok(pedidos);
        }

        // Obtiene la información completa de un pedido.
        // Administradores y Empleados pueden ver cualquiera.
        // El Cliente solamente puede ver los propios.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            // Busca el pedido solicitado.
            var pedido = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.IdPedido == id)
                .Select(p => new
                {
                    // Información general.
                    idPedido = p.IdPedido,
                    idUsuario = p.IdUsuario,
                    fechaPedido = p.FechaPedido,
                    estado = p.Estado,
                    subtotal = p.Subtotal,
                    impuesto = p.Impuesto,
                    descuento = p.Descuento,
                    total = p.Total,
                    direccionEntrega = p.DireccionEntrega,
                    idEstadoPedido = p.IdEstadoPedido,

                    // Determina el estado del pago.
                    estadoPago =
                        p.Estado == "Cancelado"
                            ? "Cancelado"
                            : p.Pagos
                                .OrderByDescending(pago => pago.IdPago)
                                .Select(pago => pago.Estado)
                                .FirstOrDefault() == "Aprobado"
                                    ? "Pagado"
                                    : "Pendiente",

                    // Obtiene los datos del último pago.
                    metodoPago = p.Pagos
                        .OrderByDescending(pago => pago.IdPago)
                        .Select(pago => pago.MetodoPago)
                        .FirstOrDefault(),

                    fechaPago = p.Pagos
                        .OrderByDescending(pago => pago.IdPago)
                        .Select(pago => pago.FechaPago)
                        .FirstOrDefault(),

                    // Indica si el pedido todavía puede pagarse.
                    puedePagar =
                        p.Estado != "Cancelado" &&
                        p.Pagos
                            .OrderByDescending(pago => pago.IdPago)
                            .Select(pago => pago.Estado)
                            .FirstOrDefault() != "Aprobado",

                    // Indica si el pedido puede ser cancelado.
                    puedeCancelar =
                        (p.Estado == "Pendiente" ||
                         p.Estado == "Confirmado") &&
                        !p.Pagos.Any(
                            pago => pago.Estado == "Aprobado"),

                    // Obtiene los productos incluidos en el pedido.
                    detalles = p.DetallePedidos
                        .Select(detalle => new
                        {
                            idDetallePedido = detalle.IdDetallePedido,
                            idProducto = detalle.IdProducto,

                            // Obtiene el nombre del producto relacionado.
                            nombreProducto =
                                detalle.IdProductoNavigation.Nombre,

                            cantidad = detalle.Cantidad,
                            precioUnitario = detalle.PrecioUnitario,
                            descuento = detalle.Descuento,
                            impuesto = detalle.Impuesto,
                            subtotal = detalle.Subtotal
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            // Comprueba si el pedido existe.
            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }

            // Obtiene el rol del usuario autenticado.
            var rol = User.FindFirstValue(ClaimTypes.Role);

            // Administradores y Empleados pueden consultar cualquier pedido.
            if (rol == "Administrador" || rol == "Empleado")
            {
                return Ok(pedido);
            }

            // Obtiene el ID del usuario autenticado.
            var idUsuarioTexto =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario del token.");
            }

            // Evita que un cliente consulte pedidos de otro usuario.
            if (pedido.idUsuario != idUsuario)
            {
                return Forbid();
            }

            return Ok(pedido);
        }

        // Crea un nuevo pedido utilizando el servicio de negocio.
        [Authorize(Roles = "Cliente")]
        [HttpPost("confirmar")]
        public async Task<ActionResult<PedidoCreadoDto>>
            ConfirmarPedido([FromBody] PedidoCrearDto pedidoDto)
        {
            // Obtiene el ID del cliente desde el token.
            var idUsuarioTexto =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Valida el ID del usuario.
            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario del token.");
            }

            // Envía la información al servicio encargado de crear el pedido.
            var resultado =
                await _pedidoServicio.CrearPedidoAsync(
                    idUsuario,
                    pedidoDto);

            return Ok(resultado);
        }

        // Cancela un pedido perteneciente al cliente autenticado.
        [Authorize(Roles = "Cliente")]
        [HttpPut("{id:int}/cancelar")]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            // Obtiene el ID del cliente desde el token.
            var idUsuarioTexto =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario del token.");
            }

            // Inicia una transacción para asegurar que
            // todos los cambios se realicen correctamente.
            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Busca el pedido perteneciente al cliente.
                var pedido = await _context.Pedidos
                    .FirstOrDefaultAsync(
                        p => p.IdPedido == id &&
                             p.IdUsuario == idUsuario);

                if (pedido == null)
                {
                    return NotFound(
                        "El pedido no existe o no pertenece al usuario autenticado.");
                }

                // Evita cancelar un pedido que ya está cancelado.
                if (pedido.Estado == "Cancelado")
                {
                    return BadRequest(
                        "El pedido ya se encuentra cancelado.");
                }

                // Comprueba si el pedido ya tiene un pago aprobado.
                var pagoAprobado = await _context.Pagos
                    .AnyAsync(
                        pago => pago.IdPedido == id &&
                                pago.Estado == "Aprobado");

                if (pagoAprobado)
                {
                    return BadRequest(
                        "No se puede cancelar un pedido que ya fue pagado.");
                }

                // Solo permite cancelar pedidos pendientes o confirmados.
                if (pedido.Estado != "Pendiente" &&
                    pedido.Estado != "Confirmado")
                {
                    return BadRequest(
                        $"No se puede cancelar un pedido con estado {pedido.Estado}.");
                }

                // Obtiene los productos del pedido.
                var detalles = await _context.DetallePedidos
                    .Where(d => d.IdPedido == id)
                    .ToListAsync();

                // Devuelve las cantidades al inventario.
                foreach (var detalle in detalles)
                {
                    var inventario = await _context.Inventarios
                        .FirstOrDefaultAsync(
                            i => i.IdProducto == detalle.IdProducto);

                    if (inventario != null)
                    {
                        inventario.CantidadDisponible +=
                            detalle.Cantidad;

                        inventario.FechaActualizacion =
                            DateTime.UtcNow;
                    }
                }

                // Busca el estado Cancelado activo.
                var estadoCancelado =
                    await _context.EstadoPedidos
                        .FirstOrDefaultAsync(
                            e => e.Nombre == "Cancelado" &&
                                 e.Estado);

                if (estadoCancelado == null)
                {
                    await transaccion.RollbackAsync();

                    return BadRequest(
                        "No existe un estado Cancelado activo en la base de datos.");
                }

                // Actualiza el estado del pedido.
                pedido.IdEstadoPedido =
                    estadoCancelado.IdEstadoPedido;

                pedido.Estado =
                    estadoCancelado.Nombre;

                // Guarda los cambios.
                await _context.SaveChangesAsync();

                // Confirma la transacción.
                await transaccion.CommitAsync();

                return Ok(new
                {
                    mensaje = "Pedido cancelado correctamente.",
                    idPedido = pedido.IdPedido,
                    estado = pedido.Estado
                });
            }
            catch (Exception)
            {
                // Revierte los cambios si ocurre algún error.
                await transaccion.RollbackAsync();

                return StatusCode(
                    500,
                    "Ocurrió un error al cancelar el pedido.");
            }
        }

        // Permite a Administradores y Empleados cambiar el estado.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstadoPedido(
            int id,
            [FromBody] CambiarEstadoPedidoDto dto)
        {
            // Comprueba que se haya enviado un estado válido.
            if (dto.IdEstadoPedido <= 0)
            {
                return BadRequest(
                    "Debe indicar un estado de pedido válido.");
            }

            // Busca el pedido.
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }

            // Busca el estado activo indicado.
            var nuevoEstado = await _context.EstadoPedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e => e.IdEstadoPedido == dto.IdEstadoPedido &&
                         e.Estado);

            if (nuevoEstado == null)
            {
                return BadRequest(
                    "El estado indicado no existe o está inactivo.");
            }

            // Evita actualizar al mismo estado.
            if (pedido.IdEstadoPedido ==
                nuevoEstado.IdEstadoPedido)
            {
                return BadRequest(
                    $"El pedido ya tiene el estado {nuevoEstado.Nombre}.");
            }

            // Actualiza el estado del pedido.
            pedido.IdEstadoPedido =
                nuevoEstado.IdEstadoPedido;

            pedido.Estado =
                nuevoEstado.Nombre;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje =
                    "Estado del pedido actualizado correctamente.",
                idPedido = pedido.IdPedido,
                idEstadoPedido = pedido.IdEstadoPedido,
                estado = pedido.Estado
            });
        }

        // Elimina un pedido.
        // Solo los Administradores tienen permiso.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            // Busca el pedido que se desea eliminar.
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }

            // Comprueba si el pedido tiene productos relacionados.
            var tieneDetalles = await _context.DetallePedidos
                .AnyAsync(d => d.IdPedido == id);

            // No permite eliminar pedidos con detalles.
            if (tieneDetalles)
            {
                return BadRequest(
                    "No se puede eliminar el pedido porque tiene detalles registrados.");
            }

            // Elimina el pedido.
            _context.Pedidos.Remove(pedido);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO utilizado para cambiar el estado de un pedido.
    public class CambiarEstadoPedidoDto
    {
        // Identificador del nuevo estado.
        public int IdEstadoPedido
        {
            get;
            set;
        }
    }
}