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

// Importa las entidades de la base de datos,
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
        // ASP.NET Core proporciona automáticamente estas dependencias
        // mediante el sistema de inyección de dependencias.
        public PedidosController(
            TiendaOnlineContext context,
            IPedidoServicio pedidoServicio)
        {
            // Guarda el contexto recibido en la variable privada.
            _context = context;

            // Guarda el servicio de pedidos recibido.
            _pedidoServicio = pedidoServicio;
        }


        // =========================================================
        // OBTENER TODOS LOS PEDIDOS
        // =========================================================

        // GET: api/Pedidos
        // Permite consultar todos los pedidos del sistema.
        // Solamente Administrador y Empleado pueden utilizarlo.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            // Consulta la tabla Pedido.
            var pedidos = await _context.Pedidos

                // AsNoTracking se utiliza porque solamente
                // se van a consultar los registros.
                .AsNoTracking()

                // Ordena primero los pedidos más recientes.
                .OrderByDescending(
                    p => p.FechaPedido
                )

                // Ejecuta la consulta y convierte
                // los resultados en una lista.
                .ToListAsync();


            // Devuelve código HTTP 200
            // junto con la lista de pedidos.
            return Ok(pedidos);
        }


        // =========================================================
        // OBTENER LOS PEDIDOS DEL CLIENTE
        // =========================================================

        // GET: api/Pedidos/mis-pedidos
        // Permite que un Cliente consulte únicamente
        // los pedidos que le pertenecen.
        [Authorize(Roles = "Cliente")]
        [HttpGet("mis-pedidos")]
        public async Task<IActionResult> GetMisPedidos()
        {
            // Obtiene del token JWT el identificador
            // del usuario que actualmente inició sesión.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Intenta convertir el identificador
            // recibido desde el token a un número entero.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // Si no puede obtenerse correctamente el usuario,
                // devuelve una respuesta HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Consulta los pedidos que pertenecen
            // únicamente al usuario autenticado.
            var pedidos = await _context.Pedidos

                // No se necesita realizar seguimiento de cambios
                // porque solamente se van a leer los datos.
                .AsNoTracking()

                // Filtra los pedidos utilizando el IdUsuario
                // obtenido anteriormente desde el token.
                .Where(
                    p => p.IdUsuario == idUsuario
                )

                // Ordena primero las compras más recientes.
                .OrderByDescending(
                    p => p.FechaPedido
                )

                // Select permite escoger únicamente
                // los campos que necesita la pantalla Mis Pedidos.
                //
                // De esta manera no se devuelve toda la entidad Pedido
                // con sus propiedades de navegación.
                .Select(
                    p => new
                    {
                        // Identificador único del pedido.
                        idPedido =
                            p.IdPedido,

                        // Fecha en que se realizó la compra.
                        fechaPedido =
                            p.FechaPedido,

                        // Estado actual del pedido.
                        estado =
                            p.Estado,

                        // Subtotal de la compra.
                        subtotal =
                            p.Subtotal,

                        // Monto correspondiente al impuesto.
                        impuesto =
                            p.Impuesto,

                        // Monto correspondiente al descuento.
                        descuento =
                            p.Descuento,

                        // Total final de la compra.
                        total =
                            p.Total,

                        // Dirección donde se entregará el pedido.
                        direccionEntrega =
                            p.DireccionEntrega
                    }
                )

                // Ejecuta la consulta en SQL Server
                // y convierte el resultado en una lista.
                .ToListAsync();


            // Devuelve código HTTP 200
            // junto con los pedidos encontrados.
            return Ok(pedidos);
        }


        // =========================================================
        // OBTENER UN PEDIDO ESPECÍFICO
        // =========================================================

        // GET: api/Pedidos/5
        //
        // Administrador y Empleado pueden consultar cualquier pedido.
        // Un Cliente solamente puede consultar un pedido que le pertenezca.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Pedido>> GetPedido(
            int id
        )
        {
            // Busca en la base de datos
            // el pedido correspondiente al identificador recibido.
            var pedido = await _context.Pedidos

                // Solamente se consultará el registro.
                .AsNoTracking()

                // Busca el primer pedido que tenga ese IdPedido.
                .FirstOrDefaultAsync(
                    p => p.IdPedido == id
                );


            // Comprueba si el pedido fue encontrado.
            if (pedido == null)
            {
                // Si no existe, devuelve código HTTP 404.
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Obtiene desde el token
            // el rol del usuario autenticado.
            var rol =
                User.FindFirstValue(
                    ClaimTypes.Role
                );


            // Los Administradores y Empleados
            // pueden consultar cualquier pedido.
            if (
                rol == "Administrador" ||
                rol == "Empleado"
            )
            {
                // Devuelve el pedido encontrado.
                return Ok(pedido);
            }


            // Si el usuario es Cliente,
            // obtiene su identificador desde el token.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Intenta convertir el identificador
            // del usuario a número entero.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // Si no se logra identificar al usuario,
                // devuelve HTTP 401.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Comprueba que el pedido solicitado
            // realmente pertenezca al Cliente.
            if (pedido.IdUsuario != idUsuario)
            {
                // Si intenta consultar un pedido de otra persona,
                // devuelve HTTP 403 Forbidden.
                return Forbid();
            }


            // Si el pedido pertenece al usuario,
            // devuelve el registro.
            return Ok(pedido);
        }


        // =========================================================
        // CONFIRMAR UNA COMPRA
        // =========================================================

        // POST: api/Pedidos/confirmar
        //
        // Este endpoint crea un pedido nuevo
        // para el Cliente que inició sesión.
        [Authorize(Roles = "Cliente")]
        [HttpPost("confirmar")]
        public async Task<ActionResult<PedidoCreadoDto>>
            ConfirmarPedido(
                [FromBody] PedidoCrearDto pedidoDto
            )
        {
            // Obtiene desde el token
            // el IdUsuario del Cliente.
            var idUsuarioTexto =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            // Intenta convertir el identificador
            // a un número entero.
            if (
                !int.TryParse(
                    idUsuarioTexto,
                    out int idUsuario
                )
            )
            {
                // No permite registrar el pedido
                // si no puede identificar al usuario.
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }


            // Envía la información al servicio de pedidos.
            //
            // El servicio contiene la lógica necesaria
            // para registrar realmente la compra.
            var resultado =
                await _pedidoServicio.CrearPedidoAsync(
                    idUsuario,
                    pedidoDto
                );


            // Devuelve el resultado obtenido
            // mediante código HTTP 200.
            return Ok(resultado);
        }


        // =========================================================
        // CAMBIAR EL ESTADO DE UN PEDIDO
        // =========================================================

        // PUT: api/Pedidos/5/estado
        //
        // Permite modificar únicamente el estado del pedido.
        //
        // No modifica subtotal, impuesto,
        // descuento, total ni productos.
        //
        // Solamente Administrador y Empleado
        // pueden utilizar este endpoint.
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
                // Devuelve HTTP 400
                // si el identificador no es válido.
                return BadRequest(
                    "Debe indicar un estado de pedido válido."
                );
            }


            // Busca el pedido que se desea modificar.
            var pedido =
                await _context.Pedidos
                    .FirstOrDefaultAsync(
                        p => p.IdPedido == id
                    );


            // Comprueba que el pedido exista.
            if (pedido == null)
            {
                // Devuelve HTTP 404
                // si el pedido no fue encontrado.
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Busca en EstadoPedido
            // el nuevo estado seleccionado.
            var nuevoEstado =
                await _context.EstadoPedidos

                    // Solamente se va a consultar el estado.
                    .AsNoTracking()

                    // Busca por identificador y además
                    // comprueba que esté activo.
                    .FirstOrDefaultAsync(
                        e =>
                            e.IdEstadoPedido ==
                            dto.IdEstadoPedido &&
                            e.Estado
                    );


            // Comprueba que el estado solicitado exista.
            if (nuevoEstado == null)
            {
                // Devuelve HTTP 400 si el estado
                // no existe o está desactivado.
                return BadRequest(
                    "El estado indicado no existe o está inactivo."
                );
            }


            // Comprueba si el pedido
            // ya tiene exactamente ese mismo estado.
            if (
                pedido.IdEstadoPedido ==
                nuevoEstado.IdEstadoPedido
            )
            {
                // No realiza una modificación innecesaria.
                return BadRequest(
                    $"El pedido ya tiene el estado {nuevoEstado.Nombre}."
                );
            }


            // Actualiza el identificador
            // del estado del pedido.
            pedido.IdEstadoPedido =
                nuevoEstado.IdEstadoPedido;

            // Actualiza también el nombre
            // del estado guardado en Pedido.
            pedido.Estado =
                nuevoEstado.Nombre;


            // Guarda los cambios
            // permanentemente en SQL Server.
            await _context.SaveChangesAsync();


            // Devuelve información
            // sobre el cambio realizado.
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

                    // Nombre del estado nuevo.
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
        // Solamente el Administrador
        // puede eliminar un pedido.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePedido(
            int id
        )
        {
            // Busca el pedido
            // mediante el identificador recibido.
            var pedido =
                await _context.Pedidos
                    .FirstOrDefaultAsync(
                        p => p.IdPedido == id
                    );


            // Comprueba si existe.
            if (pedido == null)
            {
                // Devuelve HTTP 404
                // cuando no encuentra el pedido.
                return NotFound(
                    "El pedido no existe."
                );
            }


            // Comprueba si existe al menos
            // un detalle asociado a ese pedido.
            var tieneDetalles =
                await _context.DetallePedidos

                    // AnyAsync devuelve true
                    // cuando encuentra al menos un registro.
                    .AnyAsync(
                        d => d.IdPedido == id
                    );


            // No permite eliminar pedidos
            // que ya contienen productos registrados.
            if (tieneDetalles)
            {
                return BadRequest(
                    "No se puede eliminar el pedido porque tiene detalles registrados."
                );
            }


            // Marca el pedido para ser eliminado.
            _context.Pedidos.Remove(
                pedido
            );


            // Guarda el cambio
            // en la base de datos.
            await _context.SaveChangesAsync();


            // HTTP 204 indica que la eliminación
            // fue realizada correctamente
            // y no es necesario devolver contenido.
            return NoContent();
        }
    }


    // =============================================================
    // DTO PARA CAMBIAR EL ESTADO DEL PEDIDO
    // =============================================================

    // Esta clase representa la información
    // que debe recibirse para modificar
    // el estado de un pedido.
    public class CambiarEstadoPedidoDto
    {
        // Guarda el identificador
        // del nuevo EstadoPedido.
        public int IdEstadoPedido { get; set; }
    }
}