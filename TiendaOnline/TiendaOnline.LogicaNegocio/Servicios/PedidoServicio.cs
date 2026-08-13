// Permite trabajar con Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa los DTO utilizados por el servicio.
using TiendaOnline.Dominio.DTO;

// Importa las entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

// Importa la interfaz del servicio.
using TiendaOnline.Dominio.InterfacesLN;

// Define el espacio de nombres.
namespace TiendaOnline.LogicaNegocio.Servicios;

// Implementa la creación de pedidos.
public class PedidoServicio : IPedidoServicio
{
    // Guarda el contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto.
    public PedidoServicio(
        TiendaOnlineContext context
    )
    {
        // Guarda el contexto recibido.
        _context = context;
    }

    // Crea un nuevo pedido.
    public async Task<PedidoCreadoDto> CrearPedidoAsync(
        int idUsuario,
        PedidoCrearDto pedidoDto
    )
    {
        // Valida el usuario.
        if (idUsuario <= 0)
        {
            throw new ArgumentException(
                "El usuario no es válido."
            );
        }

        // Comprueba que existan productos.
        if (
            pedidoDto.Detalles == null ||
            pedidoDto.Detalles.Count == 0
        )
        {
            throw new ArgumentException(
                "El pedido debe contener al menos un producto."
            );
        }

        // Comprueba la dirección.
        if (
            string.IsNullOrWhiteSpace(
                pedidoDto.DireccionEntrega
            )
        )
        {
            throw new ArgumentException(
                "Debe indicar una dirección de entrega."
            );
        }

        // Comprueba que el usuario exista.
        var usuarioExiste =
            await _context.Usuarios
                .AnyAsync(
                    usuario =>
                        usuario.IdUsuario == idUsuario &&
                        usuario.Estado
                );

        // Detiene si el usuario no existe.
        if (!usuarioExiste)
        {
            throw new KeyNotFoundException(
                "El usuario no existe o está inactivo."
            );
        }

        // Agrupa productos repetidos.
        var detallesAgrupados =
            pedidoDto.Detalles
                .GroupBy(
                    detalle =>
                        detalle.IdProducto
                )
                .Select(
                    grupo =>
                        new DetallePedidoCrearDto
                        {
                            // Guarda el producto.
                            IdProducto =
                                grupo.Key,

                            // Suma las cantidades.
                            Cantidad =
                                grupo.Sum(
                                    detalle =>
                                        detalle.Cantidad
                                )
                        }
                )
                .OrderBy(
                    detalle =>
                        detalle.IdProducto
                )
                .ToList();

        // Valida las cantidades.
        if (
            detallesAgrupados.Any(
                detalle =>
                    detalle.Cantidad <= 0
            )
        )
        {
            throw new ArgumentException(
                "Todas las cantidades deben ser mayores que cero."
            );
        }

        // Calcula la cantidad total.
        var cantidadTotal =
            detallesAgrupados.Sum(
                detalle =>
                    detalle.Cantidad
            );

        // Obtiene la promoción seleccionada.
        var promocion =
            ObtenerPromocionPermitida(
                pedidoDto.IdPromocion,
                cantidadTotal
            );

        // Obtiene el descuento.
        var porcentajeDescuento =
            promocion?.Porcentaje ?? 0;

        // Inicia una transacción.
        await using var transaccion =
            await _context.Database
                .BeginTransactionAsync();

        try
        {
            // Busca el estado Pendiente.
            var estadoPendiente =
                await _context.EstadoPedidos
                    .FirstOrDefaultAsync(
                        estado =>
                            estado.Nombre == "Pendiente" &&
                            estado.Estado
                    );

            // Comprueba que exista.
            if (estadoPendiente == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado Pendiente activo."
                );
            }

            // Acumula el subtotal.
            decimal subtotalPedido = 0;

            // Acumula el impuesto.
            decimal impuestoPedido = 0;

            // Acumula el descuento.
            decimal descuentoPedido = 0;

            // Guarda los cálculos de cada producto.
            var lineasCalculadas =
                new List<LineaPedidoCalculada>();

            // Recorre los productos.
            foreach (
                var detalleDto in detallesAgrupados
            )
            {
                // Busca el producto.
                var producto =
                    await _context.Productos

                        // Solo lectura.
                        .AsNoTracking()

                        // Busca producto activo.
                        .FirstOrDefaultAsync(
                            producto =>
                                producto.IdProducto ==
                                detalleDto.IdProducto &&
                                producto.Estado
                        );

                // Comprueba que exista.
                if (producto == null)
                {
                    throw new KeyNotFoundException(
                        $"El producto con ID {detalleDto.IdProducto} " +
                        "no existe o está inactivo."
                    );
                }

                // Consulta el inventario actual.
                var inventario =
                    await _context.Inventarios

                        // Solo se consulta.
                        .AsNoTracking()

                        // Busca por producto.
                        .FirstOrDefaultAsync(
                            inventario =>
                                inventario.IdProducto ==
                                producto.IdProducto
                        );

                // Comprueba que exista inventario.
                if (inventario == null)
                {
                    throw new InvalidOperationException(
                        $"El producto {producto.Nombre} " +
                        "no tiene inventario registrado."
                    );
                }

                // Comprueba el stock actual.
                if (
                    inventario.CantidadDisponible <
                    detalleDto.Cantidad
                )
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente para {producto.Nombre}. " +
                        $"Disponible: {inventario.CantidadDisponible}. " +
                        $"Solicitado: {detalleDto.Cantidad}."
                    );
                }

                // IMPORTANTE:
                // Aquí NO se descuenta inventario.
                // Se descontará cuando el pedido sea pagado.

                // Busca el impuesto.
                var impuesto =
                    await _context.Impuestos

                        // Solo lectura.
                        .AsNoTracking()

                        // Busca el impuesto activo.
                        .FirstOrDefaultAsync(
                            impuesto =>
                                impuesto.IdImpuesto ==
                                producto.IdImpuesto &&
                                impuesto.Estado
                        );

                // Comprueba el impuesto.
                if (impuesto == null)
                {
                    throw new InvalidOperationException(
                        $"El producto {producto.Nombre} " +
                        "no tiene un impuesto activo."
                    );
                }

                // Calcula el subtotal.
                var subtotalLinea =
                    Math.Round(
                        producto.Precio *
                        detalleDto.Cantidad,
                        2
                    );

                // Calcula el descuento.
                var descuentoLinea =
                    Math.Round(
                        subtotalLinea *
                        porcentajeDescuento /
                        100,
                        2
                    );

                // Calcula la base imponible.
                var baseImponible =
                    subtotalLinea -
                    descuentoLinea;

                // Calcula el impuesto.
                var impuestoLinea =
                    Math.Round(
                        baseImponible *
                        impuesto.Porcentaje /
                        100,
                        2
                    );

                // Suma el subtotal.
                subtotalPedido +=
                    subtotalLinea;

                // Suma el descuento.
                descuentoPedido +=
                    descuentoLinea;

                // Suma el impuesto.
                impuestoPedido +=
                    impuestoLinea;

                // Guarda los cálculos.
                lineasCalculadas.Add(
                    new LineaPedidoCalculada
                    {
                        // Guarda el producto.
                        Producto =
                            producto,

                        // Guarda la cantidad.
                        Cantidad =
                            detalleDto.Cantidad,

                        // Guarda el precio.
                        PrecioUnitario =
                            producto.Precio,

                        // Guarda el subtotal.
                        Subtotal =
                            subtotalLinea,

                        // Guarda el descuento.
                        Descuento =
                            descuentoLinea,

                        // Guarda el impuesto.
                        Impuesto =
                            impuestoLinea
                    }
                );
            }

            // Redondea el subtotal.
            subtotalPedido =
                Math.Round(
                    subtotalPedido,
                    2
                );

            // Redondea el descuento.
            descuentoPedido =
                Math.Round(
                    descuentoPedido,
                    2
                );

            // Redondea el impuesto.
            impuestoPedido =
                Math.Round(
                    impuestoPedido,
                    2
                );

            // Calcula el total.
            var totalPedido =
                Math.Round(
                    subtotalPedido -
                    descuentoPedido +
                    impuestoPedido,
                    2
                );

            // Crea el pedido.
            var pedido =
                new Pedido
                {
                    // Guarda el cliente.
                    IdUsuario =
                        idUsuario,

                    // Guarda la fecha.
                    FechaPedido =
                        DateTime.Now,

                    // Inicia Pendiente.
                    Estado =
                        "Pendiente",

                    // Guarda el subtotal.
                    Subtotal =
                        subtotalPedido,

                    // Guarda el impuesto.
                    Impuesto =
                        impuestoPedido,

                    // Guarda el descuento.
                    Descuento =
                        descuentoPedido,

                    // Guarda el total.
                    Total =
                        totalPedido,

                    // Guarda la dirección.
                    DireccionEntrega =
                        pedidoDto
                            .DireccionEntrega?
                            .Trim(),

                    // Relaciona el estado.
                    IdEstadoPedido =
                        estadoPendiente
                            .IdEstadoPedido
                };

            // Agrega el pedido.
            _context.Pedidos.Add(
                pedido
            );

            // Guarda para obtener el id.
            await _context
                .SaveChangesAsync();

            // Recorre los productos.
            foreach (
                var linea in lineasCalculadas
            )
            {
                // Crea el detalle.
                var detallePedido =
                    new DetallePedido
                    {
                        // Relaciona el pedido.
                        IdPedido =
                            pedido.IdPedido,

                        // Relaciona el producto.
                        IdProducto =
                            linea.Producto
                                .IdProducto,

                        // Guarda la cantidad.
                        Cantidad =
                            linea.Cantidad,

                        // Guarda el precio.
                        PrecioUnitario =
                            linea.PrecioUnitario,

                        // Guarda el descuento.
                        Descuento =
                            linea.Descuento,

                        // Guarda el impuesto.
                        Impuesto =
                            linea.Impuesto,

                        // Guarda el subtotal.
                        Subtotal =
                            linea.Subtotal
                    };

                // Agrega el detalle.
                _context.DetallePedidos.Add(
                    detallePedido
                );

            }

            // Guarda los detalles.
            await _context
                .SaveChangesAsync();

            // Confirma la transacción.
            await transaccion
                .CommitAsync();

            // Devuelve el pedido creado.
            return new PedidoCreadoDto
            {
                // Devuelve el id.
                IdPedido =
                    pedido.IdPedido,

                // Devuelve la fecha.
                FechaPedido =
                    pedido.FechaPedido,

                // Devuelve el subtotal.
                Subtotal =
                    pedido.Subtotal,

                // Devuelve el impuesto.
                Impuesto =
                    pedido.Impuesto,

                // Devuelve el descuento.
                Descuento =
                    pedido.Descuento,

                // Devuelve el total.
                Total =
                    pedido.Total,

                // Devuelve el estado.
                Estado =
                    pedido.Estado,

                // Devuelve el mensaje.
                Mensaje =
                    "Pedido creado correctamente."
            };
        }
        catch
        {
            // Revierte los cambios.
            await transaccion
                .RollbackAsync();

            // Lanza nuevamente el error.
            throw;
        }
    }

    // Valida una promoción.
    private PromocionPermitida? ObtenerPromocionPermitida(
        int? idPromocion,
        int cantidadTotal
    )
    {
        // No aplica promoción.
        if (
            !idPromocion.HasValue ||
            idPromocion.Value == 0
        )
        {
            return null;
        }

        // Define las promociones.
        var promociones =
            new List<PromocionPermitida>
            {
                // Promoción de 5%.
                new PromocionPermitida
                {
                    Id = 1,
                    Nombre = "Compra Esencial",
                    CantidadMinima = 2,
                    Porcentaje = 5
                },

                // Promoción de 10%.
                new PromocionPermitida
                {
                    Id = 2,
                    Nombre = "Rutina de Cuidado",
                    CantidadMinima = 5,
                    Porcentaje = 10
                },

                // Promoción de 15%.
                new PromocionPermitida
                {
                    Id = 3,
                    Nombre = "Cliente Frecuente",
                    CantidadMinima = 10,
                    Porcentaje = 15
                },

                // Promoción de 20%.
                new PromocionPermitida
                {
                    Id = 4,
                    Nombre = "Compra Especial",
                    CantidadMinima = 20,
                    Porcentaje = 20
                },

                // Promoción de 30%.
                new PromocionPermitida
                {
                    Id = 5,
                    Nombre = "Compra Mayorista",
                    CantidadMinima = 50,
                    Porcentaje = 30
                }
            };

        // Busca la promoción.
        var promocion =
            promociones.FirstOrDefault(
                promocion =>
                    promocion.Id ==
                    idPromocion.Value
            );

        // Comprueba que exista.
        if (promocion == null)
        {
            throw new ArgumentException(
                "La promoción seleccionada no es válida."
            );
        }

        // Comprueba la cantidad mínima.
        if (
            cantidadTotal <
            promocion.CantidadMinima
        )
        {
            throw new ArgumentException(
                $"La promoción {promocion.Nombre} requiere " +
                $"al menos {promocion.CantidadMinima} productos."
            );
        }

        // Devuelve la promoción.
        return promocion;
    }

    // Guarda las reglas de promoción.
    private class PromocionPermitida
    {
        // Guarda el id.
        public int Id { get; set; }

        // Guarda el nombre.
        public string Nombre { get; set; } =
            string.Empty;

        // Guarda la cantidad mínima.
        public int CantidadMinima { get; set; }

        // Guarda el porcentaje.
        public decimal Porcentaje { get; set; }
    }

    // Guarda cálculos de cada línea.
    private class LineaPedidoCalculada
    {
        // Guarda el producto.
        public Producto Producto { get; set; } =
            null!;

        // Guarda la cantidad.
        public int Cantidad { get; set; }

        // Guarda el precio.
        public decimal PrecioUnitario { get; set; }

        // Guarda el subtotal.
        public decimal Subtotal { get; set; }

        // Guarda el descuento.
        public decimal Descuento { get; set; }

        // Guarda el impuesto.
        public decimal Impuesto { get; set; }
    }
}