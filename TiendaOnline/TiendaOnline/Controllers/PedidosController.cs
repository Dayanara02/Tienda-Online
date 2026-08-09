// Permite utilizar atributos de autorización como [Authorize].
// Se usa para proteger los endpoints según el usuario y su rol.
using Microsoft.AspNetCore.Authorization;

// Permite crear controladores de API y devolver respuestas HTTP
// como Ok(), BadRequest(), NotFound(), Unauthorized(), etc.
using Microsoft.AspNetCore.Mvc;

// Permite utilizar funciones de Entity Framework Core,
// por ejemplo AsNoTracking(), Where(), Select() y ToListAsync().
using Microsoft.EntityFrameworkCore;

// Permite leer los datos o Claims que vienen dentro del token JWT,
// como el identificador y el rol del usuario autenticado.
using System.Security.Claims;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa los DTO utilizados por el proceso de pedidos.
using TiendaOnline.Dominio.DTO;

// Importa las entidades de la base de datos.
using TiendaOnline.Dominio.Entidades;

// Importa las interfaces de lógica de negocio.
using TiendaOnline.Dominio.InterfacesLN;


// Define el espacio de nombres donde se encuentra este controlador.
namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal del controlador.
    // Como la clase se llama PedidosController,
    // la dirección resultante será api/Pedidos.
    [Route("api/[controller]")]

    // Indica que esta clase funciona como controlador de una API.
    [ApiController]

    // Obliga a que todos los métodos de este controlador
    // requieran que el usuario haya iniciado sesión.
    [Authorize]
    public class PedidosController : ControllerBase
    {
        // Guarda el contexto de Entity Framework.
        // Se utiliza para consultar y modificar la base de datos.
        private readonly TiendaOnlineContext _context;

        // Guarda el servicio encargado de la lógica
        // utilizada para crear y confirmar pedidos.
        private readonly IPedidoServicio _pedidoServicio;


        // Constructor del controlador.
        // ASP.NET Core proporciona automáticamente estas dependencias.
        public PedidosController(
            TiendaOnlineContext context,
            IPedidoServicio pedidoServicio)
        {
            // Guarda el contexto recibido.
            _context = context;

            // Guarda el servicio de pedidos recibido.
            _pedidoServicio = pedidoServicio;
        }


        // =========================================================
        // OBTENER TODOS LOS PEDIDOS
        // =========================================================

        // GET: api/Pedidos
        //
        // Solamente Administrador y Empleado
        // pueden consultar todos los pedidos.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            // Consulta todos los pedidos.
            var pedidos = await _context.Pedidos

                // No se necesita seguimiento porque
                // solamente estamos leyendo información.
                .AsNoTracking()

                // Ordena primero los pedidos más recientes.
                .OrderByDescending(
                    p => p.FechaPedido
                )

                // Ejecuta la consulta.
                .ToListAsync();


            // Devuelve los pedidos encontrados.
            return Ok(pedidos);
        }


        // =========================================================
        // OBTENER LOS PEDIDOS DEL CLIENTE
        // =========================================================

        // GET: api/Pedidos/mis-pedidos
        //
        // Permite que el Cliente vea únicamente
        // los pedidos que le pertenecen.
        [Authorize(Roles = "Cliente")]
        [HttpGet("mis-pedidos")]
        public async Task<IActionResult> GetMisPedidos()
        {
            // Obtiene desde el token JWT
            // el identificador del usuario autenticado.
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
                // Si el token no contiene un usuario válido,
                // devuelve código HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Consulta solamente los pedidos
            // que pertenecen al Cliente autenticado.
            var pedidos = await _context.Pedidos

                // La consulta solamente será de lectura.
                .AsNoTracking()

                // Filtra utilizando el usuario del token.
                .Where(
                    p => p.IdUsuario == idUsuario
                )

                // Ordena primero las compras más recientes.
                .OrderByDescending(
                    p => p.FechaPedido
                )

                // Selecciona únicamente la información
                // que necesita la pantalla Mis Pedidos.
                .Select(
                    p => new
                    {
                        // Identificador del pedido.
                        idPedido =
                            p.IdPedido,

                        // Fecha en que se realizó.
                        fechaPedido =
                            p.FechaPedido,

                        // Estado general del pedido.
                        //
                        // Este puede ser Pendiente,
                        // Confirmado, Enviado, etc.
                        estado =
                            p.Estado,

                        // Subtotal de la compra.
                        subtotal =
                            p.Subtotal,

                        // Impuesto total.
                        impuesto =
                            p.Impuesto,

                        // Descuento total.
                        descuento =
                            p.Descuento,

                        // Total final.
                        total =
                            p.Total,

                        // Dirección de entrega.
                        direccionEntrega =
                            p.DireccionEntrega,


                        // =================================================
                        // ESTADO QUE SE MOSTRARÁ AL CLIENTE
                        // =================================================

                        // Si el pedido fue cancelado,
                        // se muestra directamente Cancelado.
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

                                    : "Pendiente"
                    }
                )

                // Ejecuta la consulta en SQL Server.
                .ToListAsync();


            // Devuelve código HTTP 200
            // junto con el historial del Cliente.
            return Ok(pedidos);
        }


        // =========================================================
        // OBTENER EL DETALLE DE UN PEDIDO
        // =========================================================

        // GET: api/Pedidos/5
        //
        // Este endpoint devuelve:
        //
        // - Información general del pedido.
        // - Estado del pago.
        // - Indicación de si puede pagarse.
        // - Productos comprados.
        // - Nombre de cada producto.
        // - Cantidad.
        // - Precio unitario.
        // - Subtotal.
        //
        // Administrador y Empleado pueden consultar cualquier pedido.
        // El Cliente solamente puede consultar los suyos.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPedido(
            int id
        )
        {
            // Consulta el pedido solicitado.
            //
            // En lugar de devolver directamente toda la entidad,
            // utilizamos Select para devolver exactamente
            // la información que necesita Angular.
            var pedido = await _context.Pedidos

                // Esta consulta es únicamente de lectura.
                .AsNoTracking()

                // Busca el pedido por su identificador.
                .Where(
                    p => p.IdPedido == id
                )

                // Construye el resultado.
                .Select(
                    p => new
                    {
                        // =============================================
                        // DATOS GENERALES DEL PEDIDO
                        // =============================================

                        // Identificador del pedido.
                        idPedido =
                            p.IdPedido,

                        // Usuario propietario del pedido.
                        //
                        // Este dato se utiliza también
                        // para controlar los permisos.
                        idUsuario =
                            p.IdUsuario,

                        // Fecha de la compra.
                        fechaPedido =
                            p.FechaPedido,

                        // Estado general del pedido.
                        estado =
                            p.Estado,

                        // Subtotal general.
                        subtotal =
                            p.Subtotal,

                        // Impuesto general.
                        impuesto =
                            p.Impuesto,

                        // Descuento general.
                        descuento =
                            p.Descuento,

                        // Total final.
                        total =
                            p.Total,

                        // Dirección para entregar la compra.
                        direccionEntrega =
                            p.DireccionEntrega,

                        // Identificador del estado general.
                        idEstadoPedido =
                            p.IdEstadoPedido,


                        // =============================================
                        // ESTADO DEL PAGO PARA MOSTRAR EN LA PANTALLA
                        // =============================================

                        // Se convierte el estado interno del pago
                        // en un texto sencillo para el Cliente.
                        //
                        // Pedido cancelado:
                        //     Cancelado
                        //
                        // Pago aprobado:
                        //     Pagado
                        //
                        // Cualquier otro caso:
                        //     Pendiente
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


                        // =============================================
                        // MÉTODO DE PAGO
                        // =============================================

                        // Busca el método utilizado en el pago más reciente.
                        //
                        // Si todavía no se ha pagado,
                        // este valor puede ser null.
                        metodoPago =
                            p.Pagos
                                .OrderByDescending(
                                    pago => pago.IdPago
                                )
                                .Select(
                                    pago => pago.MetodoPago
                                )
                                .FirstOrDefault(),


                        // =============================================
                        // FECHA DEL PAGO
                        // =============================================

                        // Obtiene la fecha del pago más reciente.
                        //
                        // Si todavía no existe un pago,
                        // se devolverá null.
                        fechaPago =
                            p.Pagos
                                .OrderByDescending(
                                    pago => pago.IdPago
                                )
                                .Select(
                                    pago => pago.FechaPago
                                )
                                .FirstOrDefault(),


                        // =============================================
                        // INDICAR SI PUEDE PAGARSE
                        // =============================================

                        // El botón Pagar solamente debe aparecer
                        // cuando:
                        //
                        // 1. El pedido NO está cancelado.
                        // 2. No existe todavía un pago aprobado.
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


                        // =============================================
                        // PRODUCTOS DEL PEDIDO
                        // =============================================

                        // Recorre todos los detalles
                        // que pertenecen al pedido.
                        detalles =
                            p.DetallePedidos

                                // Selecciona la información
                                // que necesitamos mostrar.
                                .Select(
                                    detalle => new
                                    {
                                        // Identificador del detalle.
                                        idDetallePedido =
                                            detalle.IdDetallePedido,

                                        // Identificador del producto.
                                        idProducto =
                                            detalle.IdProducto,

                                        // Obtiene el nombre real del producto
                                        // utilizando la relación con Producto.
                                        nombreProducto =
                                            detalle
                                                .IdProductoNavigation
                                                .Nombre,

                                        // Cantidad comprada.
                                        cantidad =
                                            detalle.Cantidad,

                                        // Precio de una unidad
                                        // en el momento de realizar la compra.
                                        precioUnitario =
                                            detalle.PrecioUnitario,

                                        // Descuento aplicado al producto.
                                        descuento =
                                            detalle.Descuento,

                                        // Impuesto aplicado al producto.
                                        impuesto =
                                            detalle.Impuesto,

                                        // Subtotal correspondiente
                                        // a este producto.
                                        subtotal =
                                            detalle.Subtotal
                                    }
                                )

                                // Convierte los productos
                                // en una lista.
                                .ToList()
                    }
                )

                // Obtiene solamente un pedido.
                .FirstOrDefaultAsync();


            // Comprueba si el pedido existe.
            if (pedido == null)
            {
                // Si no existe,
                // devuelve código HTTP 404.
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Obtiene el rol desde el token JWT.
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
                // Devuelve todo el detalle.
                return Ok(pedido);
            }


            // Obtiene el identificador
            // del Cliente autenticado.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Comprueba que sea un número válido.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // Si no puede identificar al Cliente,
                // devuelve HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Comprueba que el pedido realmente
            // pertenezca al Cliente autenticado.
            if (pedido.idUsuario != idUsuario)
            {
                // Un Cliente no puede consultar
                // pedidos pertenecientes a otra persona.
                return Forbid();
            }


            // Si todo está correcto,
            // devuelve el pedido con sus productos.
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
            // del Cliente desde el token.
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
                // No permite crear el pedido
                // si no puede identificar al Cliente.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Envía el pedido al servicio
            // donde se encuentra la lógica de negocio.
            var resultado =
                await _pedidoServicio.CrearPedidoAsync(
                    idUsuario,
                    pedidoDto
                );


            // Devuelve el pedido creado.
            return Ok(resultado);
        }


        // =========================================================
        // CAMBIAR EL ESTADO DE UN PEDIDO
        // =========================================================

        // PUT: api/Pedidos/5/estado
        //
        // Solamente Administrador y Empleado
        // pueden cambiar el estado general del pedido.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstadoPedido(
            int id,
            [FromBody] CambiarEstadoPedidoDto dto
        )
        {
            // Verifica que se haya recibido
            // un identificador de estado válido.
            if (dto.IdEstadoPedido <= 0)
            {
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
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Busca el nuevo estado.
            var nuevoEstado =
                await _context.EstadoPedidos

                    // Solamente consulta información.
                    .AsNoTracking()

                    // Busca el estado solicitado
                    // y comprueba que esté activo.
                    .FirstOrDefaultAsync(
                        e =>
                            e.IdEstadoPedido ==
                            dto.IdEstadoPedido &&
                            e.Estado
                    );


            // Comprueba que el estado exista.
            if (nuevoEstado == null)
            {
                return BadRequest(
                    "El estado indicado no existe o está inactivo."
                );
            }


            // Evita realizar una actualización innecesaria.
            if (
                pedido.IdEstadoPedido ==
                nuevoEstado.IdEstadoPedido
            )
            {
                return BadRequest(
                    $"El pedido ya tiene el estado {nuevoEstado.Nombre}."
                );
            }


            // Actualiza el identificador del estado.
            pedido.IdEstadoPedido =
                nuevoEstado.IdEstadoPedido;

            // Actualiza también el nombre guardado
            // directamente en Pedido.
            pedido.Estado =
                nuevoEstado.Nombre;


            // Guarda los cambios.
            await _context.SaveChangesAsync();


            // Devuelve una confirmación.
            return Ok(
                new
                {
                    mensaje =
                        "Estado del pedido actualizado correctamente.",

                    idPedido =
                        pedido.IdPedido,

                    idEstadoPedido =
                        pedido.IdEstadoPedido,

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
        // Solamente el Administrador puede eliminar pedidos.
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
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Comprueba si el pedido
            // ya contiene productos.
            var tieneDetalles =
                await _context.DetallePedidos
                    .AnyAsync(
                        d => d.IdPedido == id
                    );


            // No permite eliminar pedidos
            // que ya tengan detalles asociados.
            if (tieneDetalles)
            {
                return BadRequest(
                    "No se puede eliminar el pedido porque tiene detalles registrados."
                );
            }


            // Marca el pedido para eliminarlo.
            _context.Pedidos.Remove(
                pedido
            );


            // Guarda la eliminación.
            await _context.SaveChangesAsync();


            // Devuelve HTTP 204.
            return NoContent();
        }
    }


    // =============================================================
    // DTO PARA CAMBIAR EL ESTADO DEL PEDIDO
    // =============================================================

    // Representa la información necesaria
    // para cambiar el estado de un pedido.
    public class CambiarEstadoPedidoDto
    {
        // Identificador del nuevo EstadoPedido.
        public int IdEstadoPedido { get; set; }
    }
}