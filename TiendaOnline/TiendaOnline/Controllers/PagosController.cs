// Permite utilizar atributos como [Authorize]
// para controlar quién puede utilizar cada endpoint.
using Microsoft.AspNetCore.Authorization;

// Permite crear controladores de API y devolver
// respuestas como Ok(), BadRequest(), NotFound(), etc.
using Microsoft.AspNetCore.Mvc;

// Permite realizar consultas con Entity Framework Core,
// por ejemplo FirstOrDefaultAsync(), AnyAsync() y AsNoTracking().
using Microsoft.EntityFrameworkCore;

// Permite leer los datos almacenados dentro del token JWT,
// principalmente el identificador y el rol del usuario.
using System.Security.Claims;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades como Pago.
using TiendaOnline.Dominio.Entidades;


// Define el espacio de nombres del controlador.
namespace TiendaOnline.API.Controllers;


// Indica que todos los endpoints de este controlador
// requieren que el usuario haya iniciado sesión.
[Authorize]

// Indica que esta clase funciona como controlador de API.
[ApiController]

// Define la dirección principal:
//
// api/Pagos
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    // Guarda el contexto de Entity Framework
    // utilizado para trabajar con SQL Server.
    private readonly TiendaOnlineContext _context;


    // Constructor del controlador.
    public PagosController(
        TiendaOnlineContext context
    )
    {
        // Guarda el contexto recibido mediante
        // inyección de dependencias.
        _context = context;
    }


    // =========================================================
    // OBTENER TODOS LOS PAGOS
    // =========================================================

    // GET: api/Pagos
    //
    // Solamente Administrador y Empleado
    // pueden consultar todos los pagos del sistema.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pago>>>
        GetPagos()
    {
        // Consulta todos los pagos.
        var pagos =
            await _context.Pagos

                // No se necesita seguimiento porque
                // solamente se va a leer información.
                .AsNoTracking()

                // Ordena primero los pagos más recientes.
                .OrderByDescending(
                    p => p.IdPago
                )

                // Ejecuta la consulta.
                .ToListAsync();


        // Devuelve código HTTP 200
        // junto con los pagos encontrados.
        return Ok(pagos);
    }


    // =========================================================
    // OBTENER UN PAGO
    // =========================================================

    // GET: api/Pagos/5
    //
    // Administrador y Empleado pueden consultar cualquier pago.
    // Un Cliente solamente puede consultar pagos de sus pedidos.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPago(
        int id
    )
    {
        // Busca el pago solicitado y obtiene también
        // el usuario propietario del pedido.
        var pago =
            await _context.Pagos

                // Esta consulta solamente lee información.
                .AsNoTracking()

                // Busca el pago mediante su identificador.
                .Where(
                    p => p.IdPago == id
                )

                // Devuelve únicamente la información necesaria.
                .Select(
                    p => new
                    {
                        // Identificador del pago.
                        idPago =
                            p.IdPago,

                        // Pedido relacionado.
                        idPedido =
                            p.IdPedido,

                        // Usuario propietario del pedido.
                        idUsuario =
                            p.IdPedidoNavigation.IdUsuario,

                        // Método utilizado.
                        metodoPago =
                            p.MetodoPago,

                        // Referencia del pago.
                        referencia =
                            p.Referencia,

                        // Monto pagado.
                        monto =
                            p.Monto,

                        // Fecha en que se pagó.
                        fechaPago =
                            p.FechaPago,

                        // Estado del pago.
                        estado =
                            p.Estado,

                        // Identificador del método.
                        idMetodoPago =
                            p.IdMetodoPago,

                        // Identificador del estado del pago.
                        idEstadoPago =
                            p.IdEstadoPago
                    }
                )

                // Obtiene solamente un resultado.
                .FirstOrDefaultAsync();


        // Comprueba si el pago existe.
        if (pago == null)
        {
            return NotFound(
                "El pago no existe."
            );
        }


        // Obtiene el rol del usuario autenticado.
        var rol =
            User.FindFirstValue(
                ClaimTypes.Role
            );


        // Administrador y Empleado
        // pueden consultar cualquier pago.
        if (
            rol == "Administrador" ||
            rol == "Empleado"
        )
        {
            return Ok(pago);
        }


        // Obtiene el identificador del Cliente
        // desde el token JWT.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );


        // Comprueba que el identificador
        // recibido desde el token sea válido.
        if (
            !int.TryParse(
                idUsuarioTexto,
                out int idUsuario
            )
        )
        {
            return Unauthorized(
                "No se pudo identificar al usuario del token."
            );
        }


        // El Cliente solamente puede consultar
        // pagos relacionados con sus propios pedidos.
        if (pago.idUsuario != idUsuario)
        {
            return Forbid();
        }


        // Devuelve el pago.
        return Ok(pago);
    }


    // =========================================================
    // PAGAR UN PEDIDO
    // =========================================================

    // POST: api/Pagos/pagar
    //
    // Este es el endpoint que utilizará
    // el botón "Pagar pedido" de Angular.
    //
    // Solamente los Clientes pueden utilizarlo.
    [Authorize(Roles = "Cliente")]
    [HttpPost("pagar")]
    public async Task<IActionResult> PagarPedido(
        [FromBody] PagarPedidoDto dto
    )
    {
        // =====================================================
        // 1. IDENTIFICAR AL CLIENTE
        // =====================================================

        // Obtiene desde el token JWT
        // el identificador del Cliente.
        var idUsuarioTexto =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );


        // Comprueba que pueda convertirse a número.
        if (
            !int.TryParse(
                idUsuarioTexto,
                out int idUsuario
            )
        )
        {
            return Unauthorized(
                "No se pudo identificar al usuario del token."
            );
        }


        // =====================================================
        // 2. VALIDAR LOS DATOS RECIBIDOS
        // =====================================================

        // Comprueba que se haya enviado
        // un pedido válido.
        if (dto.IdPedido <= 0)
        {
            return BadRequest(
                "Debe indicar un pedido válido."
            );
        }


        // Comprueba que se haya seleccionado
        // un método de pago.
        if (dto.IdMetodoPago <= 0)
        {
            return BadRequest(
                "Debe seleccionar un método de pago."
            );
        }


        // =====================================================
        // 3. BUSCAR EL PEDIDO
        // =====================================================

        // Busca el pedido que el Cliente desea pagar.
        var pedido =
            await _context.Pedidos
                .FirstOrDefaultAsync(
                    p => p.IdPedido == dto.IdPedido
                );


        // Comprueba que el pedido exista.
        if (pedido == null)
        {
            return NotFound(
                "El pedido no existe."
            );
        }


        // =====================================================
        // 4. COMPROBAR QUE EL PEDIDO SEA DEL CLIENTE
        // =====================================================

        // Evita que un Cliente pueda pagar
        // el pedido de otra persona.
        if (pedido.IdUsuario != idUsuario)
        {
            return Forbid();
        }


        // =====================================================
        // 5. COMPROBAR QUE NO ESTÉ CANCELADO
        // =====================================================

        // Un pedido cancelado ya no puede pagarse.
        if (pedido.Estado == "Cancelado")
        {
            return BadRequest(
                "No se puede pagar un pedido cancelado."
            );
        }


        // =====================================================
        // 6. COMPROBAR SI YA ESTÁ PAGADO
        // =====================================================

        // Busca si ya existe un pago aprobado
        // relacionado con este pedido.
        var yaEstaPagado =
            await _context.Pagos
                .AnyAsync(
                    p =>
                        p.IdPedido == pedido.IdPedido &&
                        p.Estado == "Aprobado"
                );


        // Evita cobrar dos veces el mismo pedido.
        if (yaEstaPagado)
        {
            return BadRequest(
                "Este pedido ya se encuentra pagado."
            );
        }


        // =====================================================
        // 7. BUSCAR EL MÉTODO DE PAGO
        // =====================================================

        // Busca el método seleccionado por el Cliente.
        //
        // También comprueba que el método se encuentre activo.
        var metodoPago =
            await _context.MetodoPagos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    m =>
                        m.IdMetodoPago ==
                        dto.IdMetodoPago &&
                        m.Estado
                );


        // Comprueba que el método exista.
        if (metodoPago == null)
        {
            return BadRequest(
                "El método de pago seleccionado no existe o está inactivo."
            );
        }


        // =====================================================
        // 8. BUSCAR EL ESTADO APROBADO
        // =====================================================

        // Busca en la tabla EstadoPago
        // el estado llamado Aprobado.
        var estadoPagoAprobado =
            await _context.EstadoPagos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e =>
                        e.Nombre == "Aprobado" &&
                        e.Estado
                );


        // Comprueba que ese estado exista
        // correctamente en la base de datos.
        if (estadoPagoAprobado == null)
        {
            return BadRequest(
                "No se encontró el estado de pago Aprobado."
            );
        }


        // =====================================================
        // 9. BUSCAR EL ESTADO PAGADO DEL PEDIDO
        // =====================================================

        // Busca en EstadoPedido
        // el estado general llamado Pagado.
        var estadoPedidoPagado =
            await _context.EstadoPedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e =>
                        e.Nombre == "Pagado" &&
                        e.Estado
                );


        // Comprueba que exista.
        if (estadoPedidoPagado == null)
        {
            return BadRequest(
                "No se encontró el estado de pedido Pagado."
            );
        }


        // =====================================================
        // 10. CREAR EL PAGO
        // =====================================================

        // Crea una nueva entidad Pago.
        var nuevoPago =
            new Pago
            {
                // Relaciona el pago con el pedido.
                IdPedido =
                    pedido.IdPedido,

                // Guarda también el nombre del método
                // para conservar la información del pago.
                MetodoPago =
                    metodoPago.Nombre,

                // Crea una referencia sencilla
                // para identificar esta transacción.
                Referencia =
                    $"PAGO-{pedido.IdPedido}-{DateTime.UtcNow:yyyyMMddHHmmss}",

                // IMPORTANTE:
                // El monto NO viene desde Angular.
                //
                // Se toma directamente del Total guardado
                // en el pedido para evitar modificaciones.
                Monto =
                    pedido.Total,

                // Guarda la fecha actual del pago.
                FechaPago =
                    DateTime.UtcNow,

                // Como este proyecto simula el proceso de pago,
                // el pago se registra directamente como Aprobado.
                Estado =
                    estadoPagoAprobado.Nombre,

                // Guarda la relación con MetodoPago.
                IdMetodoPago =
                    metodoPago.IdMetodoPago,

                // Guarda la relación con EstadoPago.
                IdEstadoPago =
                    estadoPagoAprobado.IdEstadoPago
            };


        // Agrega el nuevo pago al contexto.
        _context.Pagos.Add(
            nuevoPago
        );


        // =====================================================
        // 11. CAMBIAR EL PEDIDO A PAGADO
        // =====================================================

        // Actualiza el identificador
        // del estado general del pedido.
        pedido.IdEstadoPedido =
            estadoPedidoPagado.IdEstadoPedido;

        // Actualiza también el nombre
        // guardado directamente en Pedido.
        pedido.Estado =
            estadoPedidoPagado.Nombre;


        // =====================================================
        // 12. GUARDAR TODO EN LA BASE DE DATOS
        // =====================================================

        // Guarda el Pago y el cambio del Pedido
        // en SQL Server.
        await _context.SaveChangesAsync();


        // =====================================================
        // 13. DEVOLVER LA RESPUESTA
        // =====================================================

        // Devuelve la información necesaria
        // para que Angular sepa que el pago funcionó.
        return Ok(
            new
            {
                // Mensaje para mostrar al Cliente.
                mensaje =
                    "Pago realizado correctamente.",

                // Pedido que acaba de pagarse.
                idPedido =
                    pedido.IdPedido,

                // Identificador generado para el pago.
                idPago =
                    nuevoPago.IdPago,

                // Estado mostrado al Cliente.
                estadoPago =
                    "Pagado",

                // Estado general del pedido.
                estadoPedido =
                    pedido.Estado,

                // Método seleccionado.
                metodoPago =
                    nuevoPago.MetodoPago,

                // Monto realmente cobrado.
                monto =
                    nuevoPago.Monto,

                // Referencia generada.
                referencia =
                    nuevoPago.Referencia,

                // Fecha del pago.
                fechaPago =
                    nuevoPago.FechaPago
            }
        );
    }


    // =========================================================
    // MODIFICAR UN PAGO
    // =========================================================

    // PUT: api/Pagos/5
    //
    // Solamente Administrador y Empleado
    // pueden modificar manualmente un pago.
    [Authorize(Roles = "Administrador,Empleado")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutPago(
        int id,
        Pago pago
    )
    {
        // Busca el pago actual.
        var pagoActual =
            await _context.Pagos
                .FindAsync(id);


        // Comprueba que exista.
        if (pagoActual == null)
        {
            return NotFound(
                "El pago no existe."
            );
        }


        // Actualiza la información permitida.
        pagoActual.IdPedido =
            pago.IdPedido;

        pagoActual.MetodoPago =
            pago.MetodoPago;

        pagoActual.Referencia =
            pago.Referencia;

        pagoActual.Monto =
            pago.Monto;

        pagoActual.FechaPago =
            pago.FechaPago;

        pagoActual.Estado =
            pago.Estado;

        pagoActual.IdMetodoPago =
            pago.IdMetodoPago;

        pagoActual.IdEstadoPago =
            pago.IdEstadoPago;


        // Guarda los cambios.
        await _context.SaveChangesAsync();


        // HTTP 204 indica que la actualización
        // terminó correctamente.
        return NoContent();
    }


    // =========================================================
    // ELIMINAR UN PAGO
    // =========================================================

    // DELETE: api/Pagos/5
    //
    // Solamente Administrador
    // puede eliminar pagos.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePago(
        int id
    )
    {
        // Busca el pago.
        var pago =
            await _context.Pagos
                .FindAsync(id);


        // Comprueba que exista.
        if (pago == null)
        {
            return NotFound(
                "El pago no existe."
            );
        }


        // Marca el pago para eliminarlo.
        _context.Pagos.Remove(
            pago
        );


        // Guarda la eliminación.
        await _context.SaveChangesAsync();


        // Devuelve HTTP 204.
        return NoContent();
    }
}


// =============================================================
// DTO PARA PAGAR UN PEDIDO
// =============================================================

// Esta clase representa únicamente
// lo que Angular debe enviar al pagar.
// Esa información se controla desde el backend.
public class PagarPedidoDto
{
    // Pedido que se desea pagar.
    public int IdPedido { get; set; }

    // Método seleccionado por el Cliente.
    public int IdMetodoPago { get; set; }
}