using Microsoft.EntityFrameworkCore;
using System.Data;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.DTO;
using TiendaOnline.Dominio.Model;
using TiendaOnline.LogicaNegocio.Interfaces;

namespace TiendaOnline.LogicaNegocio.Servicios;

public class PedidoServicio : IPedidoServicio
{
    private readonly TiendaOnlineContext _context;

    public PedidoServicio(TiendaOnlineContext context)
    {
        _context = context;
    }

    public async Task<PedidoCreadoDto> CrearPedidoAsync(
        int idUsuario,
        PedidoCrearDto pedidoDto)
    {
        if (idUsuario <= 0)
        {
            throw new ArgumentException(
                "El usuario no es válido."
            );
        }

        if (pedidoDto.Detalles == null ||
            pedidoDto.Detalles.Count == 0)
        {
            throw new ArgumentException(
                "El pedido debe contener al menos un producto."
            );
        }

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u =>
                u.IdUsuario == idUsuario &&
                u.Estado);

        if (!usuarioExiste)
        {
            throw new KeyNotFoundException(
                "El usuario no existe o está inactivo."
            );
        }

        /*
         * Agrupa los productos repetidos.
         * Por ejemplo, si el mismo producto aparece dos veces,
         * suma las cantidades.
         */
        var detallesAgrupados = pedidoDto.Detalles
            .GroupBy(d => d.IdProducto)
            .Select(grupo => new DetallePedidoCrearDto
            {
                IdProducto = grupo.Key,
                Cantidad = grupo.Sum(d => d.Cantidad)
            })
            .OrderBy(d => d.IdProducto)
            .ToList();

        if (detallesAgrupados.Any(d => d.Cantidad <= 0))
        {
            throw new ArgumentException(
                "Todas las cantidades deben ser mayores que cero."
            );
        }

        /*
         * Serializable ayuda a evitar que dos compras simultáneas
         * utilicen el mismo inventario.
         */
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable
            );

        try
        {
            var estadoPendiente = await _context.EstadoPedidos
                .FirstOrDefaultAsync(e =>
                    e.Nombre == "Pendiente" &&
                    e.Estado);

            if (estadoPendiente == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado Pendiente activo."
                );
            }

            decimal subtotalPedido = 0;
            decimal impuestoPedido = 0;
            decimal descuentoPedido = 0;

            /*
             * Aquí guardaremos temporalmente la información
             * calculada de cada producto.
             */
            var lineasCalculadas =
                new List<LineaPedidoCalculada>();

            foreach (var detalleDto in detallesAgrupados)
            {
                var producto = await _context.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p =>
                        p.IdProducto == detalleDto.IdProducto &&
                        p.Estado);

                if (producto == null)
                {
                    throw new KeyNotFoundException(
                        $"El producto con ID {detalleDto.IdProducto} " +
                        "no existe o está inactivo."
                    );
                }

                var inventario = await _context.Inventarios
                    .FirstOrDefaultAsync(i =>
                        i.IdProducto == producto.IdProducto);

                if (inventario == null)
                {
                    throw new InvalidOperationException(
                        $"El producto {producto.Nombre} " +
                        "no tiene inventario registrado."
                    );
                }

                if (inventario.CantidadDisponible <
                    detalleDto.Cantidad)
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente para {producto.Nombre}. " +
                        $"Disponible: {inventario.CantidadDisponible}. " +
                        $"Solicitado: {detalleDto.Cantidad}."
                    );
                }

                var impuesto = await _context.Impuestos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i =>
                        i.IdImpuesto == producto.IdImpuesto &&
                        i.Estado);

                if (impuesto == null)
                {
                    throw new InvalidOperationException(
                        $"El producto {producto.Nombre} " +
                        "no tiene un impuesto activo."
                    );
                }

                var subtotalLinea = Math.Round(
                    producto.Precio * detalleDto.Cantidad,
                    2
                );

                /*
                 * Los descuentos se conectarán en el próximo paso.
                 * Por ahora se deja en cero para completar primero
                 * la transacción y evitar sobreventa.
                 */
                decimal descuentoLinea = 0;

                var baseImponible =
                    subtotalLinea - descuentoLinea;

                var impuestoLinea = Math.Round(
                    baseImponible *
                    impuesto.Porcentaje / 100,
                    2
                );

                subtotalPedido += subtotalLinea;
                descuentoPedido += descuentoLinea;
                impuestoPedido += impuestoLinea;

                lineasCalculadas.Add(
                    new LineaPedidoCalculada
                    {
                        Producto = producto,
                        Inventario = inventario,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = subtotalLinea,
                        Descuento = descuentoLinea,
                        Impuesto = impuestoLinea
                    }
                );
            }

            subtotalPedido = Math.Round(subtotalPedido, 2);
            impuestoPedido = Math.Round(impuestoPedido, 2);
            descuentoPedido = Math.Round(descuentoPedido, 2);

            var totalPedido = Math.Round(
                subtotalPedido +
                impuestoPedido -
                descuentoPedido,
                2
            );

            var pedido = new Pedido
            {
                IdUsuario = idUsuario,
                FechaPedido = DateTime.Now,
                Estado = "Pendiente",
                Subtotal = subtotalPedido,
                Impuesto = impuestoPedido,
                Descuento = descuentoPedido,
                Total = totalPedido,
                DireccionEntrega =
                    pedidoDto.DireccionEntrega?.Trim(),
                IdEstadoPedido =
                    estadoPendiente.IdEstadoPedido
            };

            _context.Pedidos.Add(pedido);

            /*
             * Primer guardado para obtener el IdPedido
             * generado por SQL Server.
             */
            await _context.SaveChangesAsync();

            foreach (var linea in lineasCalculadas)
            {
                var detallePedido = new DetallePedido
                {
                    IdPedido = pedido.IdPedido,
                    IdProducto =
                        linea.Producto.IdProducto,
                    Cantidad = linea.Cantidad,
                    PrecioUnitario =
                        linea.PrecioUnitario,
                    Descuento = linea.Descuento,
                    Impuesto = linea.Impuesto,
                    Subtotal = linea.Subtotal
                };

                _context.DetallePedidos.Add(detallePedido);

                /*
                 * Descontar inventario.
                 */
                linea.Inventario.CantidadDisponible -=
                    linea.Cantidad;

                linea.Inventario.FechaActualizacion =
                    DateTime.Now;

                /*
                 * Registrar la salida en el historial.
                 */
                var movimiento =
                    new MovimientoInventario
                    {
                        IdInventario =
                            linea.Inventario.IdInventario,

                        IdUsuario = idUsuario,

                        TipoMovimiento = "Salida",

                        Cantidad = linea.Cantidad,

                        Motivo =
                            $"Venta del pedido #{pedido.IdPedido}",

                        FechaMovimiento = DateTime.Now
                    };

                _context.MovimientoInventarios.Add(
                    movimiento
                );
            }

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return new PedidoCreadoDto
            {
                IdPedido = pedido.IdPedido,
                FechaPedido = pedido.FechaPedido,
                Subtotal = pedido.Subtotal,
                Impuesto = pedido.Impuesto,
                Descuento = pedido.Descuento,
                Total = pedido.Total,
                Estado = pedido.Estado,
                Mensaje =
                    "Pedido creado y existencias actualizadas correctamente."
            };
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;
        }
    }

    /*
     * Clase interna utilizada únicamente para almacenar
     * los cálculos temporales de cada producto.
     */
    private class LineaPedidoCalculada
    {
        public Producto Producto { get; set; } = null!;

        public Inventario Inventario { get; set; } = null!;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Descuento { get; set; }

        public decimal Impuesto { get; set; }
    }
}