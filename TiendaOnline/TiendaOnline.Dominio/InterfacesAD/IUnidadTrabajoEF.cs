using System;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.Dominio.InterfacesAD
{
    // Esta interfaz define todos los repositorios que va a manejar
    // la unidad de trabajo dentro del proyecto.
    public interface IUnidadTrabajoEF : IDisposable
    {
        // Permite realizar operaciones de base de datos con los usuarios.
        IRepositorioAD<Usuario> TUsuario { get; }

        // Permite realizar operaciones con los roles del sistema.
        IRepositorioAD<Rol> TRol { get; }

        // Permite realizar operaciones con los productos.
        IRepositorioAD<Producto> TProducto { get; }

        // Permite realizar operaciones con las categorías.
        IRepositorioAD<Categorium> TCategoria { get; }

        // Permite realizar operaciones con los pedidos.
        IRepositorioAD<Pedido> TPedido { get; }

        // Permite trabajar con el detalle de cada pedido.
        IRepositorioAD<DetallePedido> TDetallePedido { get; }

        // Permite trabajar con el inventario de los productos.
        IRepositorioAD<Inventario> TInventario { get; }

        // Permite guardar y consultar movimientos realizados en inventario.
        IRepositorioAD<MovimientoInventario> TMovimientoInventario { get; }

        // Permite realizar operaciones con los descuentos.
        IRepositorioAD<Descuento> TDescuento { get; }

        // Permite administrar los proveedores.
        IRepositorioAD<Proveedor> TProveedor { get; }

        // Permite trabajar con la relación entre productos y proveedores.
        IRepositorioAD<ProductoProveedor> TProductoProveedor { get; }

        // Permite trabajar con los carritos de compra.
        IRepositorioAD<Carrito> TCarrito { get; }

        // Permite trabajar con los productos agregados a cada carrito.
        IRepositorioAD<DetalleCarrito> TDetalleCarrito { get; }

        // Permite trabajar con las listas de deseos.
        IRepositorioAD<ListaDeseo> TListaDeseo { get; }

        // Permite trabajar con los productos guardados en una lista de deseos.
        IRepositorioAD<DetalleListaDeseo> TDetalleListaDeseo { get; }

        // Permite consultar y administrar los métodos de pago.
        IRepositorioAD<MetodoPago> TMetodoPago { get; }

        // Permite trabajar con los pagos realizados.
        IRepositorioAD<Pago> TPago { get; }

        // Permite consultar los estados disponibles para los pagos.
        IRepositorioAD<EstadoPago> TEstadoPago { get; }

        // Permite consultar los estados disponibles para los pedidos.
        IRepositorioAD<EstadoPedido> TEstadoPedido { get; }

        // Permite trabajar con la información de los envíos.
        IRepositorioAD<Envio> TEnvio { get; }

        // Permite trabajar con las direcciones registradas por los usuarios.
        IRepositorioAD<DireccionUsuario> TDireccionUsuario { get; }

        // Permite realizar operaciones con las facturas.
        IRepositorioAD<Factura> TFactura { get; }

        // Permite trabajar con las evaluaciones hechas a los productos.
        IRepositorioAD<EvaluacionProducto> TEvaluacionProducto { get; }

        // Permite trabajar con los impuestos registrados.
        IRepositorioAD<Impuesto> TImpuesto { get; }

        // Permite trabajar con las familias de productos.
        IRepositorioAD<FamiliaProducto> TFamiliaProducto { get; }

        // Permite trabajar con las compras hechas a proveedores.
        IRepositorioAD<CompraProveedor> TCompraProveedor { get; }

        // Permite trabajar con el detalle de las compras a proveedores.
        IRepositorioAD<DetalleCompraProveedor> TDetalleCompraProveedor { get; }

        // Permite trabajar con las proformas.
        IRepositorioAD<Proforma> TProforma { get; }

        // Permite trabajar con el detalle de las proformas.
        IRepositorioAD<DetalleProforma> TDetalleProforma { get; }

        // Permite trabajar con las notificaciones del sistema.
        IRepositorioAD<Notificacion> TNotificacion { get; }

        // Permite consultar el historial de accesos de los usuarios.
        IRepositorioAD<HistorialAcceso> THistorialAcceso { get; }

        // Permite trabajar con los registros de la bitácora del sistema.
        IRepositorioAD<BitacoraSistema> TBitacoraSistema { get; }

        // Guarda en la base de datos todos los cambios pendientes
        // y devuelve la cantidad de registros afectados.
        int Completar();

        // Guarda los cambios pendientes y confirma la transacción actual.
        void CompletarTran();

        // Inicia una transacción para agrupar varias operaciones.
        void EmpezarTransaccion();

        // Revierte la transacción actual si ocurre algún problema.
        void Rollback();

        // Cierra la conexión del contexto con la base de datos.
        void CerrarConexion();
    }
}