// Permite trabajar con Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Permite utilizar el nivel de aislamiento de la transacción.
using System.Data;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa los DTO utilizados por el servicio.
using TiendaOnline.Dominio.DTO;

// Importa las entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

// Importa la interfaz que implementa este servicio.
using TiendaOnline.Dominio.InterfacesLN;

// Define el espacio de nombres del servicio.
namespace TiendaOnline.LogicaNegocio.Servicios;

// Implementa la lógica de negocio relacionada con pedidos.
public class PedidoServicio : IPedidoServicio
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto por inyección de dependencias.
    public PedidoServicio(
        TiendaOnlineContext context
    )
    {
        // Guarda el contexto recibido.
        _context = context;
    }

    // Crea un pedido nuevo para el cliente autenticado.
    public async Task<PedidoCreadoDto> CrearPedidoAsync(
        int idUsuario,
        PedidoCrearDto pedidoDto
    )
    {
        // Comprueba que el usuario recibido sea válido.
        if (idUsuario <= 0)
        {
            throw new ArgumentException(
                "El usuario no es válido."
            );
        }

        // Comprueba que el pedido tenga productos.
        if (
            pedidoDto.Detalles == null ||
            pedidoDto.Detalles.Count == 0
        )
        {
            throw new ArgumentException(
                "El pedido debe contener al menos un producto."
            );
        }

        // Comprueba que exista una dirección de entrega.
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

        // Comprueba que el usuario exista y esté activo.
        var usuarioExiste =
            await _context.Usuarios
                .AnyAsync(
                    usuario =>
                        usuario.IdUsuario == idUsuario &&
                        usuario.Estado
                );

        // Detiene el proceso si el usuario no es válido.
        if (!usuarioExiste)
        {
            throw new KeyNotFoundException(
                "El usuario no existe o está inactivo."
            );
        }

        // Agrupa productos repetidos y suma sus cantidades.
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
                            // Guarda el producto agrupado.
                            IdProducto =
                                grupo.Key,

                            // Suma todas sus cantidades.
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

        // Comprueba que todas las cantidades sean mayores que cero.
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

        // Calcula la cantidad total de artículos del pedido.
        var cantidadTotal =
            detallesAgrupados.Sum(
                detalle =>
                    detalle.Cantidad
            );

        // Obtiene y valida la promoción seleccionada.
        var promocion =
            ObtenerPromocionPermitida(
                pedidoDto.IdPromocion,
                cantidadTotal
            );

        // Obtiene el porcentaje autorizado por el backend.
        var porcentajeDescuento =
            promocion?.Porcentaje ?? 0;

        // Inicia una transacción para proteger pedido e inventario.
        await using var transaccion =
            await _context.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable
                );

        try
        {
            // Busca el estado Pendiente activo.
            var estadoPendiente =
                await _context.EstadoPedidos
                    .FirstOrDefaultAsync(
                        estado =>
                            estado.Nombre == "Pendiente" &&
                            estado.Estado
                    );

            // Comprueba que exista el estado necesario.
            if (estadoPendiente == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado Pendiente activo."
                );
            }

            // Acumula el subtotal completo del pedido.
            decimal subtotalPedido = 0;

            // Acumula el impuesto completo del pedido.
            decimal impuestoPedido = 0;

            // Acumula el descuento completo del pedido.
            decimal descuentoPedido = 0;

            // Guarda temporalmente los cálculos de cada producto.
            var lineasCalculadas =
                new List<LineaPedidoCalculada>();

            // Recorre cada producto solicitado.
            foreach (
                var detalleDto in detallesAgrupados
            )
            {
                // Busca el producto activo en la base de datos.
                var producto =
                    await _context.Productos

                        // La consulta del producto es solamente de lectura.
                        .AsNoTracking()

                        // Busca el producto solicitado.
                        .FirstOrDefaultAsync(
                            producto =>
                                producto.IdProducto ==
                                detalleDto.IdProducto &&
                                producto.Estado
                        );

                // Comprueba que el producto exista.
                if (producto == null)
                {
                    throw new KeyNotFoundException(
                        $"El producto con ID {detalleDto.IdProducto} " +
                        "no existe o está inactivo."
                    );
                }

                // Busca el inventario correspondiente al producto.
                var inventario =
                    await _context.Inventarios
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

                // Comprueba que exista suficiente inventario.
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

                // Busca el impuesto activo del producto.
                var impuesto =
                    await _context.Impuestos

                        // La consulta es solamente de lectura.
                        .AsNoTracking()

                        // Busca el impuesto asociado.
                        .FirstOrDefaultAsync(
                            impuesto =>
                                impuesto.IdImpuesto ==
                                producto.IdImpuesto &&
                                impuesto.Estado
                        );

                // Comprueba que exista un impuesto válido.
                if (impuesto == null)
                {
                    throw new InvalidOperationException(
                        $"El producto {producto.Nombre} " +
                        "no tiene un impuesto activo."
                    );
                }

                // Calcula precio por cantidad antes de descuentos.
                var subtotalLinea =
                    Math.Round(
                        producto.Precio *
                        detalleDto.Cantidad,
                        2
                    );

                // Calcula el descuento autorizado para esta línea.
                var descuentoLinea =
                    Math.Round(
                        subtotalLinea *
                        porcentajeDescuento /
                        100,
                        2
                    );

                // Obtiene la base después de restar el descuento.
                var baseImponible =
                    subtotalLinea -
                    descuentoLinea;

                // Calcula el impuesto después del descuento.
                var impuestoLinea =
                    Math.Round(
                        baseImponible *
                        impuesto.Porcentaje /
                        100,
                        2
                    );

                // Suma el subtotal de esta línea.
                subtotalPedido +=
                    subtotalLinea;

                // Suma el descuento de esta línea.
                descuentoPedido +=
                    descuentoLinea;

                // Suma el impuesto de esta línea.
                impuestoPedido +=
                    impuestoLinea;

                // Guarda los cálculos para crear el detalle después.
                lineasCalculadas.Add(
                    new LineaPedidoCalculada
                    {
                        // Guarda el producto encontrado.
                        Producto = producto,

                        // Guarda su inventario.
                        Inventario = inventario,

                        // Guarda la cantidad comprada.
                        Cantidad =
                            detalleDto.Cantidad,

                        // Guarda el precio real de la base de datos.
                        PrecioUnitario =
                            producto.Precio,

                        // Guarda el subtotal calculado.
                        Subtotal =
                            subtotalLinea,

                        // Guarda el descuento calculado.
                        Descuento =
                            descuentoLinea,

                        // Guarda el impuesto calculado.
                        Impuesto =
                            impuestoLinea
                    }
                );
            }

            // Redondea el subtotal final.
            subtotalPedido =
                Math.Round(
                    subtotalPedido,
                    2
                );

            // Redondea el descuento final.
            descuentoPedido =
                Math.Round(
                    descuentoPedido,
                    2
                );

            // Redondea el impuesto final.
            impuestoPedido =
                Math.Round(
                    impuestoPedido,
                    2
                );

            // Calcula el monto final del pedido.
            var totalPedido =
                Math.Round(
                    subtotalPedido -
                    descuentoPedido +
                    impuestoPedido,
                    2
                );

            // Crea el encabezado principal del pedido.
            var pedido =
                new Pedido
                {
                    // Guarda el cliente propietario.
                    IdUsuario =
                        idUsuario,

                    // Guarda la fecha actual.
                    FechaPedido =
                        DateTime.Now,

                    // El pedido comienza Pendiente.
                    Estado =
                        "Pendiente",

                    // Guarda el subtotal calculado.
                    Subtotal =
                        subtotalPedido,

                    // Guarda el impuesto calculado.
                    Impuesto =
                        impuestoPedido,

                    // Guarda el descuento calculado.
                    Descuento =
                        descuentoPedido,

                    // Guarda el total final.
                    Total =
                        totalPedido,

                    // Guarda la dirección sin espacios sobrantes.
                    DireccionEntrega =
                        pedidoDto
                            .DireccionEntrega?
                            .Trim(),

                    // Guarda el identificador del estado Pendiente.
                    IdEstadoPedido =
                        estadoPendiente
                            .IdEstadoPedido
                };

            // Agrega el pedido al contexto.
            _context.Pedidos.Add(
                pedido
            );

            // Guarda para obtener el IdPedido generado.
            await _context
                .SaveChangesAsync();

            // Recorre las líneas calculadas.
            foreach (
                var linea in lineasCalculadas
            )
            {
                // Crea el detalle correspondiente al producto.
                var detallePedido =
                    new DetallePedido
                    {
                        // Relaciona el detalle con el pedido.
                        IdPedido =
                            pedido.IdPedido,

                        // Guarda el producto comprado.
                        IdProducto =
                            linea.Producto
                                .IdProducto,

                        // Guarda la cantidad comprada.
                        Cantidad =
                            linea.Cantidad,

                        // Guarda el precio unitario real.
                        PrecioUnitario =
                            linea.PrecioUnitario,

                        // Guarda el descuento de esta línea.
                        Descuento =
                            linea.Descuento,

                        // Guarda el impuesto de esta línea.
                        Impuesto =
                            linea.Impuesto,

                        // Guarda el subtotal de esta línea.
                        Subtotal =
                            linea.Subtotal
                    };

                // Agrega el detalle a la base de datos.
                _context.DetallePedidos.Add(
                    detallePedido
                );

                // Descuenta las unidades compradas del inventario.
                linea.Inventario
                    .CantidadDisponible -=
                    linea.Cantidad;

                // Actualiza la fecha del inventario.
                linea.Inventario
                    .FechaActualizacion =
                    DateTime.Now;

                // Crea el movimiento de salida del inventario.
                var movimiento =
                    new MovimientoInventario
                    {
                        // Relaciona el movimiento con el inventario.
                        IdInventario =
                            linea.Inventario
                                .IdInventario,

                        // Guarda el usuario que realizó la compra.
                        IdUsuario =
                            idUsuario,

                        // Indica que las unidades están saliendo.
                        TipoMovimiento =
                            "Salida",

                        // Guarda la cantidad retirada.
                        Cantidad =
                            linea.Cantidad,

                        // Explica por qué cambió el inventario.
                        Motivo =
                            $"Venta del pedido #{pedido.IdPedido}",

                        // Guarda la fecha del movimiento.
                        FechaMovimiento =
                            DateTime.Now
                    };

                // Registra el movimiento de inventario.
                _context.MovimientoInventarios.Add(
                    movimiento
                );
            }

            // Guarda detalles e inventario.
            await _context
                .SaveChangesAsync();

            // Confirma todos los cambios de la transacción.
            await transaccion
                .CommitAsync();

            // Devuelve la información del pedido creado.
            return new PedidoCreadoDto
            {
                // Devuelve el identificador generado.
                IdPedido =
                    pedido.IdPedido,

                // Devuelve la fecha del pedido.
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

                // Devuelve el estado inicial.
                Estado =
                    pedido.Estado,

                // Devuelve un mensaje de confirmación.
                Mensaje =
                    "Pedido creado correctamente."
            };
        }
        catch
        {
            // Revierte todos los cambios si ocurre un error.
            await transaccion
                .RollbackAsync();

            // Vuelve a lanzar el error para que la API lo maneje.
            throw;
        }
    }

    // Valida la promoción elegida y devuelve sus reglas reales.
    private PromocionPermitida? ObtenerPromocionPermitida(
        int? idPromocion,
        int cantidadTotal
    )
    {
        // No aplica descuento cuando no se seleccionó promoción.
        if (
            !idPromocion.HasValue ||
            idPromocion.Value == 0
        )
        {
            return null;
        }

        // Define las promociones autorizadas por el sistema.
        var promociones =
            new List<PromocionPermitida>
            {
                // Requiere 2 productos y aplica 5%.
                new PromocionPermitida
                {
                    Id = 1,
                    Nombre = "Compra Esencial",
                    CantidadMinima = 2,
                    Porcentaje = 5
                },

                // Requiere 5 productos y aplica 10%.
                new PromocionPermitida
                {
                    Id = 2,
                    Nombre = "Rutina de Cuidado",
                    CantidadMinima = 5,
                    Porcentaje = 10
                },

                // Requiere 10 productos y aplica 15%.
                new PromocionPermitida
                {
                    Id = 3,
                    Nombre = "Cliente Frecuente",
                    CantidadMinima = 10,
                    Porcentaje = 15
                },

                // Requiere 20 productos y aplica 20%.
                new PromocionPermitida
                {
                    Id = 4,
                    Nombre = "Compra Especial",
                    CantidadMinima = 20,
                    Porcentaje = 20
                },

                // Requiere 50 productos y aplica 30%.
                new PromocionPermitida
                {
                    Id = 5,
                    Nombre = "Compra Mayorista",
                    CantidadMinima = 50,
                    Porcentaje = 30
                }
            };

        // Busca la promoción enviada por Angular.
        var promocion =
            promociones.FirstOrDefault(
                promocion =>
                    promocion.Id ==
                    idPromocion.Value
            );

        // Rechaza identificadores de promociones inexistentes.
        if (promocion == null)
        {
            throw new ArgumentException(
                "La promoción seleccionada no es válida."
            );
        }

        // Comprueba que el pedido cumpla la cantidad mínima.
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

        // Devuelve la promoción validada.
        return promocion;
    }

    // Guarda temporalmente las reglas de una promoción.
    private class PromocionPermitida
    {
        // Guarda el identificador.
        public int Id { get; set; }

        // Guarda el nombre.
        public string Nombre { get; set; } =
            string.Empty;

        // Guarda la cantidad mínima.
        public int CantidadMinima { get; set; }

        // Guarda el porcentaje autorizado.
        public decimal Porcentaje { get; set; }
    }

    // Guarda temporalmente los cálculos de cada producto.
    private class LineaPedidoCalculada
    {
        // Guarda el producto comprado.
        public Producto Producto { get; set; } =
            null!;

        // Guarda el inventario relacionado.
        public Inventario Inventario { get; set; } =
            null!;

        // Guarda la cantidad comprada.
        public int Cantidad { get; set; }

        // Guarda el precio unitario.
        public decimal PrecioUnitario { get; set; }

        // Guarda el subtotal.
        public decimal Subtotal { get; set; }

        // Guarda el descuento.
        public decimal Descuento { get; set; }

        // Guarda el impuesto.
        public decimal Impuesto { get; set; }
    }
}