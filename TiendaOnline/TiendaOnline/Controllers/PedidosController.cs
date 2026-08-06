using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.DTO;
using TiendaOnline.Dominio.Model;
using TiendaOnline.LogicaNegocio.Interfaces;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;
        private readonly IPedidoServicio _pedidoServicio;

        public PedidosController(
            TiendaOnlineContext context,
            IPedidoServicio pedidoServicio)
        {
            _context = context;
            _pedidoServicio = pedidoServicio;
        }

        // GET: api/Pedidos
        // Administrador y Empleado pueden ver todos los pedidos.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .AsNoTracking()
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return Ok(pedidos);
        }

        // GET: api/Pedidos/mis-pedidos
        // El Cliente solamente puede ver sus propios pedidos.
        [Authorize(Roles = "Cliente")]
        [HttpGet("mis-pedidos")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetMisPedidos()
        {
            var idUsuarioTexto = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }

            var pedidos = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.IdUsuario == idUsuario)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return Ok(pedidos);
        }

        // GET: api/Pedidos/5
        // Administrador y Empleado pueden ver cualquier pedido.
        // El Cliente solamente puede ver un pedido propio.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }

            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol == "Administrador" || rol == "Empleado")
            {
                return Ok(pedido);
            }

            var idUsuarioTexto = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }

            if (pedido.IdUsuario != idUsuario)
            {
                return Forbid();
            }

            return Ok(pedido);
        }

        // POST: api/Pedidos/confirmar
        // Crea el pedido del Cliente autenticado.
        [Authorize(Roles = "Cliente")]
        [HttpPost("confirmar")]
        public async Task<ActionResult<PedidoCreadoDto>> ConfirmarPedido(
            [FromBody] PedidoCrearDto pedidoDto)
        {
            var idUsuarioTexto = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario del token."
                );
            }

            var resultado = await _pedidoServicio.CrearPedidoAsync(
                idUsuario,
                pedidoDto
            );

            return Ok(resultado);
        }

        // PUT: api/Pedidos/5/estado
        // Solamente cambia el estado del pedido.
        // No permite modificar precios, descuentos, impuestos ni total.
        [Authorize(Roles = "Administrador,Empleado")]
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstadoPedido(
            int id,
            [FromBody] CambiarEstadoPedidoDto dto)
        {
            if (dto.IdEstadoPedido <= 0)
            {
                return BadRequest(
                    "Debe indicar un estado de pedido válido."
                );
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }

            var nuevoEstado = await _context.EstadoPedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.IdEstadoPedido == dto.IdEstadoPedido &&
                    e.Estado);

            if (nuevoEstado == null)
            {
                return BadRequest(
                    "El estado indicado no existe o está inactivo."
                );
            }

            if (pedido.IdEstadoPedido == nuevoEstado.IdEstadoPedido)
            {
                return BadRequest(
                    $"El pedido ya tiene el estado {nuevoEstado.Nombre}."
                );
            }

            pedido.IdEstadoPedido = nuevoEstado.IdEstadoPedido;
            pedido.Estado = nuevoEstado.Nombre;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado del pedido actualizado correctamente.",
                idPedido = pedido.IdPedido,
                idEstadoPedido = pedido.IdEstadoPedido,
                estado = pedido.Estado
            });
        }

        // DELETE: api/Pedidos/5
        // Solamente el Administrador puede eliminar pedidos sin detalles.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }

            var tieneDetalles = await _context.DetallePedidos
                .AnyAsync(d => d.IdPedido == id);

            if (tieneDetalles)
            {
                return BadRequest(
                    "No se puede eliminar el pedido porque tiene detalles registrados."
                );
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CambiarEstadoPedidoDto
    {
        public int IdEstadoPedido { get; set; }
    }
}
