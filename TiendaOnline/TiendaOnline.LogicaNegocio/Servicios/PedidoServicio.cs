using Microsoft.EntityFrameworkCore;
using System.Data;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.DTO;
using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Dominio.InterfacesLN;

namespace TiendaOnline.LogicaNegocio.Servicios;  // Implementa la interfaz del proyecto Dominio

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
        // Validar usuario recibido desde el token
        if (idUsuario <= 0)
        {
            throw new ArgumentException(
                "El usuario no es válido."
            );
        }

        // Validar que el pedido tenga productos
        if (pedidoDto.Detalles == null ||
            pedidoDto.Detalles.Count == 0)
        {
            throw new ArgumentException(
                "El pedido debe contener al menos un producto."
            );
        }

        // Verificar que el usuario exista y esté activo
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
         * Si el mismo producto viene repetido,
         * se agrupan sus cantidades.
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
         * La transacción Serializable ayuda a evitar
         * que dos compras usen el mismo inventario.
         */
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable
            );

        try
        {
            // Buscar el estado Pendiente
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

            var lineasCalculadas =
                new List<LineaPedidoCalculada>();

            foreach (var detalleDto in detallesAgrupados)
            {
                /*
                 * Obtener el producto con los descuentos:
                 * - asignados directamente al producto;
                 * - asignados a la categoría;
                 * - asignados a la familia.
                 */
                var producto = await _context.Productos
                    .Include(p => p.IdDescuentos)
                    .Include(p => p.IdCategoriaNavigation)
                        .ThenInclude(c => c.IdDescuentos)
                    .Include(p => p.IdCategoriaNavigation)
                        .ThenInclude(c => c.IdFamiliaNavigation)
                            .ThenInclude(f => f.IdDescuentos)
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

                // Buscar inventario del producto
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

                // Evitar vender más de lo disponible
                if (inventario.CantidadDisponible <
                    detalleDto.Cantidad)
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente para {producto.Nombre}. " +
                        $"Disponible: {inventario.CantidadDisponible}. " +
                        $"Solicitado: {detalleDto.Cantidad}."
                    );
                }

                // Buscar el impuesto del producto
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

                // Subtotal antes de descuentos e impuesto
                var subtotalLinea = Math.Round(
                    producto.Precio * detalleDto.Cantidad,
                    2
                );

                // Fecha utilizada para validar descuentos vigentes
                var hoy = DateOnly.FromDateTime(DateTime.Today);

                var descuentosAplicables =
                    new List<Descuento>();

                // Descuentos asignados al producto
                descuentosAplicables.AddRange(
                    producto.IdDescuentos
                );

                // Descuentos asignados a la categoría
                if (producto.IdCategoriaNavigation != null)
                {
                    descuentosAplicables.AddRange(
                        producto.IdCategoriaNavigation
                            .IdDescuentos
                    );
                }

                // Descuentos asignados a la familia
                if (producto.IdCategoriaNavigation?
                        .IdFamiliaNavigation != null)
                {
                    descuentosAplicables.AddRange(
                        producto.IdCategoriaNavigation
                            .IdFamiliaNavigation
                            .IdDescuentos
                    );
                }

                /*
                 * Se aplica únicamente el porcentaje más alto.
                 * No se suman los descuentos.
                 */
                var porcentajeDescuento =
                    descuentosAplicables
                        .Where(d =>
                            d.Estado &&
                            d.FechaInicio <= hoy &&
                            d.FechaFin >= hoy)
                        .Select(d => d.Porcentaje)
                        .DefaultIfEmpty(0)
                        .Max();

                var descuentoLinea = Math.Round(
                    subtotalLinea *
                    porcentajeDescuento / 100,
                    2
                );

                // El impuesto se calcula después del descuento
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

            subtotalPedido = Math.Round(
                subtotalPedido,
                2
            );

            descuentoPedido = Math.Round(
                descuentoPedido,
                2
            );

            impuestoPedido = Math.Round(
                impuestoPedido,
                2
            );

            var totalPedido = Math.Round(
                subtotalPedido -
                descuentoPedido +
                impuestoPedido,
                2
            );

            // Crear encabezado del pedido
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
             * Se guarda primero para obtener el IdPedido
             * generado por SQL Server.
             */
            await _context.SaveChangesAsync();

            foreach (var linea in lineasCalculadas)
            {
                // Crear detalle del pedido
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

                _context.DetallePedidos.Add(
                    detallePedido
                );

                // Descontar inventario
                linea.Inventario.CantidadDisponible -=
                    linea.Cantidad;

                linea.Inventario.FechaActualizacion =
                    DateTime.Now;

                // Registrar movimiento de salida
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

            // Confirmar todos los cambios
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
                    "Pedido creado, descuento aplicado y existencias actualizadas correctamente."
            };
        }
        catch
        {
            // Si falla una operación, se revierte todo
            await transaccion.RollbackAsync();
            throw;
        }
    }

    /*
     * Clase interna para guardar temporalmente
     * los cálculos de cada producto.
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