// Permite utilizar atributos de autorización como [Authorize].
using Microsoft.AspNetCore.Authorization;

// Permite crear controladores de API y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Permite utilizar Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Permite leer los Claims contenidos en el token JWT.
using System.Security.Claims;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa los DTO relacionados con pedidos.
using TiendaOnline.Dominio.DTO;

// Importa las entidades de la base de datos.
using TiendaOnline.Dominio.Entidades;

// Importa las interfaces de lógica de negocio.
using TiendaOnline.Dominio.InterfacesLN;


// Define el espacio de nombres del controlador.
namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal:
    // api/Pedidos
    [Route("api/[controller]")]

    // Indica que esta clase funciona como API Controller.
    [ApiController]

    // Obliga a que todos los endpoints
    // requieran autenticación.
    [Authorize]
    public class PedidosController : ControllerBase
    {
        // Contexto utilizado para consultar
        // y modificar la base de datos.
        private readonly TiendaOnlineContext _context;

        // Servicio utilizado para crear pedidos.
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


        // =========================================================
        // OBTENER TODOS LOS PEDIDOS
        // =========================================================

        // GET: api/Pedidos
        //
        // Solo Administrador y Empleado
        // pueden consultar todos los pedidos.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            // Consulta todos los pedidos.
            var pedidos = await _context.Pedidos

                // Como solo se leen datos,
                // no se necesita seguimiento.
                .AsNoTracking()

                // Ordena primero los más recientes.
                .OrderByDescending(
                    p => p.FechaPedido
                )

                // Ejecuta la consulta.
                .ToListAsync();


            // Devuelve HTTP 200 con los pedidos.
            return Ok(pedidos);
        }


        // =========================================================
        // OBTENER LOS PEDIDOS DEL CLIENTE
        // =========================================================

        // GET: api/Pedidos/mis-pedidos
        //
        // El Cliente solamente puede consultar
        // los pedidos que le pertenecen.
        [Authorize(Roles = "Cliente")]
        [HttpGet("mis-pedidos")]
        public async Task<IActionResult> GetMisPedidos()
        {
            // Obtiene el identificador del usuario
            // desde el token JWT.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Intenta convertir el identificador a número.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // Si no se puede identificar al usuario,
                // devuelve HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Consulta solamente los pedidos
            // del cliente autenticado.
            var pedidos = await _context.Pedidos

                // La consulta es solamente de lectura.
                .AsNoTracking()

                // Filtra por el usuario autenticado.
                .Where(
                    p => p.IdUsuario == idUsuario
                )

                // Ordena primero los más recientes.
                .OrderByDescending(
                    p => p.FechaPedido
                )

                // Selecciona solamente
                // los datos que necesita Angular.
                .Select(
                    p => new
                    {
                        // Identificador del pedido.
                        idPedido =
                            p.IdPedido,

                        // Fecha del pedido.
                        fechaPedido =
                            p.FechaPedido,

                        // Estado general.
                        estado =
                            p.Estado,

                        // Subtotal.
                        subtotal =
                            p.Subtotal,

                        // Impuesto.
                        impuesto =
                            p.Impuesto,

                        // Descuento.
                        descuento =
                            p.Descuento,

                        // Total final.
                        total =
                            p.Total,

                        // Dirección de entrega.
                        direccionEntrega =
                            p.DireccionEntrega,


                        // Estado del pago mostrado al cliente.
                        estadoPago =
                            p.Estado == "Cancelado"

                                ? "Cancelado"

                                : p.Pagos
                                    .OrderByDescending(
                                        pago => pago.IdPago
                                    )
                                    .Select(
                                        pago => pago.Estado
                                    )
                                    .FirstOrDefault() == "Aprobado"

                                    ? "Pagado"

                                    : "Pendiente"
                    }
                )

                // Ejecuta la consulta.
                .ToListAsync();


            // Devuelve HTTP 200 con los pedidos.
            return Ok(pedidos);
        }


        // =========================================================
        // OBTENER EL DETALLE DE UN PEDIDO
        // =========================================================

        // GET: api/Pedidos/5
        //
        // Administrador y Empleado pueden consultar cualquier pedido.
        // El Cliente solamente puede consultar sus propios pedidos.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPedido(
            int id
        )
        {
            // Busca el pedido solicitado.
            var pedido = await _context.Pedidos

                // La consulta es solamente de lectura.
                .AsNoTracking()

                // Busca por identificador.
                .Where(
                    p => p.IdPedido == id
                )

                // Construye el resultado que recibirá Angular.
                .Select(
                    p => new
                    {
                        // =================================================
                        // INFORMACIÓN GENERAL
                        // =================================================

                        // Identificador del pedido.
                        idPedido =
                            p.IdPedido,

                        // Usuario propietario del pedido.
                        idUsuario =
                            p.IdUsuario,

                        // Fecha en que se creó.
                        fechaPedido =
                            p.FechaPedido,

                        // Estado general.
                        estado =
                            p.Estado,

                        // Subtotal.
                        subtotal =
                            p.Subtotal,

                        // Impuesto.
                        impuesto =
                            p.Impuesto,

                        // Descuento.
                        descuento =
                            p.Descuento,

                        // Total final.
                        total =
                            p.Total,

                        // Dirección de entrega.
                        direccionEntrega =
                            p.DireccionEntrega,

                        // Identificador del estado.
                        idEstadoPedido =
                            p.IdEstadoPedido,


                        // =================================================
                        // ESTADO DEL PAGO
                        // =================================================

                        // Si el pedido está cancelado,
                        // se muestra Cancelado.
                        //
                        // Si existe un pago aprobado,
                        // se muestra Pagado.
                        //
                        // En cualquier otro caso,
                        // se muestra Pendiente.
                        estadoPago =
                            p.Estado == "Cancelado"

                                ? "Cancelado"

                                : p.Pagos
                                    .OrderByDescending(
                                        pago => pago.IdPago
                                    )
                                    .Select(
                                        pago => pago.Estado
                                    )
                                    .FirstOrDefault() == "Aprobado"

                                    ? "Pagado"

                                    : "Pendiente",


                        // =================================================
                        // MÉTODO DE PAGO
                        // =================================================

                        // Obtiene el método utilizado
                        // en el pago más reciente.
                        metodoPago =
                            p.Pagos
                                .OrderByDescending(
                                    pago => pago.IdPago
                                )
                                .Select(
                                    pago => pago.MetodoPago
                                )
                                .FirstOrDefault(),


                        // =================================================
                        // FECHA DEL PAGO
                        // =================================================

                        // Obtiene la fecha
                        // del pago más reciente.
                        fechaPago =
                            p.Pagos
                                .OrderByDescending(
                                    pago => pago.IdPago
                                )
                                .Select(
                                    pago => pago.FechaPago
                                )
                                .FirstOrDefault(),


                        // =================================================
                        // INDICAR SI PUEDE PAGARSE
                        // =================================================

                        // Solo puede pagarse cuando:
                        //
                        // 1. El pedido no está cancelado.
                        // 2. No existe un pago aprobado.
                        puedePagar =
                            p.Estado != "Cancelado" &&

                            p.Pagos
                                .OrderByDescending(
                                    pago => pago.IdPago
                                )
                                .Select(
                                    pago => pago.Estado
                                )
                                .FirstOrDefault() != "Aprobado",


                        // =================================================
                        // INDICAR SI PUEDE CANCELARSE
                        // =================================================

                        // Solo puede cancelarse cuando:
                        //
                        // 1. Está Pendiente o Confirmado.
                        // 2. No existe un pago aprobado.
                        puedeCancelar =
                            (
                                p.Estado == "Pendiente" ||
                                p.Estado == "Confirmado"
                            ) &&

                            !p.Pagos.Any(
                                pago =>
                                    pago.Estado == "Aprobado"
                            ),


                        // =================================================
                        // PRODUCTOS DEL PEDIDO
                        // =================================================

                        // Recorre todos los detalles
                        // pertenecientes al pedido.
                        detalles =
                            p.DetallePedidos

                                // Selecciona la información
                                // necesaria de cada producto.
                                .Select(
                                    detalle => new
                                    {
                                        // Identificador del detalle.
                                        idDetallePedido =
                                            detalle.IdDetallePedido,

                                        // Identificador del producto.
                                        idProducto =
                                            detalle.IdProducto,

                                        // Nombre real del producto.
                                        nombreProducto =
                                            detalle
                                                .IdProductoNavigation
                                                .Nombre,

                                        // Cantidad comprada.
                                        cantidad =
                                            detalle.Cantidad,

                                        // Precio unitario.
                                        precioUnitario =
                                            detalle.PrecioUnitario,

                                        // Descuento aplicado.
                                        descuento =
                                            detalle.Descuento,

                                        // Impuesto aplicado.
                                        impuesto =
                                            detalle.Impuesto,

                                        // Subtotal del producto.
                                        subtotal =
                                            detalle.Subtotal
                                    }
                                )

                                // Convierte los detalles en una lista.
                                .ToList()
                    }
                )

                // Obtiene un solo resultado.
                .FirstOrDefaultAsync();


            // Comprueba si el pedido existe.
            if (pedido == null)
            {
                // Devuelve HTTP 404.
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Obtiene el rol
            // desde el token JWT.
            var rol =
                User.FindFirstValue(
                    ClaimTypes.Role
                );


            // Administrador y Empleado
            // pueden consultar cualquier pedido.
            if (
                rol == "Administrador" ||
                rol == "Empleado"
            )
            {
                // Devuelve el pedido.
                return Ok(pedido);
            }


            // Obtiene el identificador
            // del usuario autenticado.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Comprueba que el identificador sea válido.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // Devuelve HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Comprueba que el pedido
            // pertenezca al cliente autenticado.
            if (
                pedido.idUsuario != idUsuario
            )
            {
                // Impide consultar pedidos de otros clientes.
                return Forbid();
            }


            // Devuelve HTTP 200
            // con toda la información del pedido.
            return Ok(pedido);
        }


        // =========================================================
        // CONFIRMAR UNA COMPRA
        // =========================================================

        // POST: api/Pedidos/confirmar
        //
        // Permite que un Cliente cree un pedido.
        [Authorize(Roles = "Cliente")]
        [HttpPost("confirmar")]
        public async Task<ActionResult<PedidoCreadoDto>>
            ConfirmarPedido(
                [FromBody] PedidoCrearDto pedidoDto
            )
        {
            // Obtiene el identificador
            // del cliente desde el token.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Comprueba que sea válido.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // Devuelve HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Envía la creación del pedido
            // hacia la lógica de negocio.
            var resultado =
                await _pedidoServicio.CrearPedidoAsync(
                    idUsuario,
                    pedidoDto
                );


            // Devuelve el pedido creado.
            return Ok(resultado);
        }


        // =========================================================
        // CANCELAR UN PEDIDO COMO CLIENTE
        // =========================================================

        // PUT: api/Pedidos/5/cancelar
        //
        // Permite que un Cliente cancele
        // uno de sus propios pedidos.
        [Authorize(Roles = "Cliente")]
        [HttpPut("{id:int}/cancelar")]
        public async Task<IActionResult> CancelarPedido(
            int id
        )
        {
            // Obtiene el identificador
            // del cliente desde el token JWT.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Comprueba que el identificador sea válido.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // Devuelve HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Inicia una transacción.
            //
            // Esto permite que la cancelación
            // y la devolución del inventario
            // se realicen como una sola operación.
            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // Busca el pedido.
                var pedido =
                    await _context.Pedidos
                        .FirstOrDefaultAsync(
                            p =>
                                p.IdPedido == id &&
                                p.IdUsuario == idUsuario
                        );


                // Comprueba que el pedido exista
                // y pertenezca al cliente.
                if (pedido == null)
                {
                    // Devuelve HTTP 404.
                    return NotFound(
                        "El pedido no existe o no pertenece al usuario autenticado."
                    );
                }


                // Comprueba si el pedido
                // ya fue cancelado anteriormente.
                if (
                    pedido.Estado == "Cancelado"
                )
                {
                    // Devuelve un error controlado.
                    return BadRequest(
                        "El pedido ya se encuentra cancelado."
                    );
                }


                // Comprueba si existe
                // un pago aprobado.
                var pagoAprobado =
                    await _context.Pagos
                        .AnyAsync(
                            pago =>
                                pago.IdPedido == id &&
                                pago.Estado == "Aprobado"
                        );


                // No permite cancelar
                // un pedido que ya fue pagado.
                if (pagoAprobado)
                {
                    // Devuelve HTTP 400.
                    return BadRequest(
                        "No se puede cancelar un pedido que ya fue pagado."
                    );
                }


                // Solo permite cancelar
                // estados Pendiente o Confirmado.
                if (
                    pedido.Estado != "Pendiente" &&
                    pedido.Estado != "Confirmado"
                )
                {
                    // Devuelve HTTP 400.
                    return BadRequest(
                        $"No se puede cancelar un pedido con estado {pedido.Estado}."
                    );
                }


                // Busca todos los productos
                // que pertenecen al pedido.
                var detalles =
                    await _context.DetallePedidos

                        // Filtra por el pedido.
                        .Where(
                            detalle =>
                                detalle.IdPedido == id
                        )

                        // Ejecuta la consulta.
                        .ToListAsync();


                // Recorre cada producto comprado.
                foreach (
                    var detalle in detalles
                )
                {
                    // Busca el registro de inventario
                    // correspondiente al producto.
                    var inventario =
                        await _context.Inventarios
                            .FirstOrDefaultAsync(
                                inventario =>
                                    inventario.IdProducto ==
                                    detalle.IdProducto
                            );


                    // Comprueba que exista inventario.
                    if (inventario != null)
                    {
                        // Devuelve al inventario
                        // la cantidad comprada.
                        inventario.CantidadDisponible +=
                            detalle.Cantidad;


                        // Actualiza la fecha
                        // de modificación del inventario.
                        inventario.FechaActualizacion =
                            DateTime.UtcNow;
                    }
                }


                // Busca el estado Cancelado
                // dentro de la tabla EstadoPedido.
                var estadoCancelado =
                    await _context.EstadoPedidos
                        .FirstOrDefaultAsync(
                            estado =>
                                estado.Nombre == "Cancelado" &&
                                estado.Estado
                        );


                // Comprueba que el estado exista.
                if (estadoCancelado == null)
                {
                    // Cancela la transacción.
                    await transaccion
                        .RollbackAsync();


                    // Informa el problema.
                    return BadRequest(
                        "No existe un estado Cancelado activo en la base de datos."
                    );
                }


                // Actualiza el identificador
                // del estado del pedido.
                pedido.IdEstadoPedido =
                    estadoCancelado.IdEstadoPedido;


                // Actualiza el nombre
                // del estado del pedido.
                pedido.Estado =
                    estadoCancelado.Nombre;


                // Guarda todos los cambios
                // en la base de datos.
                await _context
                    .SaveChangesAsync();


                // Confirma la transacción.
                await transaccion
                    .CommitAsync();


                // Devuelve una respuesta de éxito.
                return Ok(
                    new
                    {
                        // Mensaje para Angular.
                        mensaje =
                            "Pedido cancelado correctamente.",

                        // Identificador cancelado.
                        idPedido =
                            pedido.IdPedido,

                        // Nuevo estado.
                        estado =
                            pedido.Estado
                    }
                );
            }
            catch (Exception)
            {
                // Si ocurre cualquier problema,
                // revierte todos los cambios.
                await transaccion
                    .RollbackAsync();


                // Devuelve un error HTTP 500.
                return StatusCode(
                    500,
                    "Ocurrió un error al cancelar el pedido."
                );
            }
        }


        // =========================================================
        // CAMBIAR EL ESTADO DE UN PEDIDO
        // =========================================================

        // PUT: api/Pedidos/5/estado
        //
        // Solo Administrador y Empleado
        // pueden cambiar el estado general.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstadoPedido(
            int id,
            [FromBody] CambiarEstadoPedidoDto dto
        )
        {
            // Comprueba que se haya enviado
            // un estado válido.
            if (
                dto.IdEstadoPedido <= 0
            )
            {
                // Devuelve HTTP 400.
                return BadRequest(
                    "Debe indicar un estado de pedido válido."
                );
            }


            // Busca el pedido.
            var pedido =
                await _context.Pedidos
                    .FirstOrDefaultAsync(
                        p => p.IdPedido == id
                    );


            // Comprueba que exista.
            if (pedido == null)
            {
                // Devuelve HTTP 404.
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Busca el nuevo estado.
            var nuevoEstado =
                await _context.EstadoPedidos

                    // La consulta es de lectura.
                    .AsNoTracking()

                    // Busca el estado activo.
                    .FirstOrDefaultAsync(
                        e =>
                            e.IdEstadoPedido ==
                            dto.IdEstadoPedido &&
                            e.Estado
                    );


            // Comprueba que el estado exista.
            if (nuevoEstado == null)
            {
                // Devuelve HTTP 400.
                return BadRequest(
                    "El estado indicado no existe o está inactivo."
                );
            }


            // Evita guardar el mismo estado.
            if (
                pedido.IdEstadoPedido ==
                nuevoEstado.IdEstadoPedido
            )
            {
                // Devuelve un mensaje controlado.
                return BadRequest(
                    $"El pedido ya tiene el estado {nuevoEstado.Nombre}."
                );
            }


            // Actualiza el identificador.
            pedido.IdEstadoPedido =
                nuevoEstado.IdEstadoPedido;


            // Actualiza el nombre.
            pedido.Estado =
                nuevoEstado.Nombre;


            // Guarda los cambios.
            await _context
                .SaveChangesAsync();


            // Devuelve el resultado.
            return Ok(
                new
                {
                    // Mensaje de confirmación.
                    mensaje =
                        "Estado del pedido actualizado correctamente.",

                    // Identificador del pedido.
                    idPedido =
                        pedido.IdPedido,

                    // Identificador del nuevo estado.
                    idEstadoPedido =
                        pedido.IdEstadoPedido,

                    // Nombre del nuevo estado.
                    estado =
                        pedido.Estado
                }
            );
        }


        // =========================================================
        // ELIMINAR UN PEDIDO
        // =========================================================

        // DELETE: api/Pedidos/5
        //
        // Solo Administrador puede eliminar pedidos.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePedido(
            int id
        )
        {
            // Busca el pedido.
            var pedido =
                await _context.Pedidos
                    .FirstOrDefaultAsync(
                        p => p.IdPedido == id
                    );


            // Comprueba que exista.
            if (pedido == null)
            {
                // Devuelve HTTP 404.
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Comprueba si tiene detalles asociados.
            var tieneDetalles =
                await _context.DetallePedidos
                    .AnyAsync(
                        d =>
                            d.IdPedido == id
                    );


            // No permite eliminar pedidos
            // que ya contienen productos.
            if (tieneDetalles)
            {
                // Devuelve HTTP 400.
                return BadRequest(
                    "No se puede eliminar el pedido porque tiene detalles registrados."
                );
            }


            // Marca el pedido para eliminar.
            _context.Pedidos.Remove(
                pedido
            );


            // Guarda la eliminación.
            await _context
                .SaveChangesAsync();


            // Devuelve HTTP 204.
            return NoContent();
        }
    }


    // =============================================================
    // DTO PARA CAMBIAR EL ESTADO DEL PEDIDO
    // =============================================================

    // Representa los datos necesarios
    // para cambiar el estado de un pedido.
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