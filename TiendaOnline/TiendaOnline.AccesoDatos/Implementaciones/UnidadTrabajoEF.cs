using Microsoft.EntityFrameworkCore.Storage; // Permite trabajar con transacciones de Entity Framework.
using Microsoft.EntityFrameworkCore; // Permite utilizar Entity Framework Core.
using TiendaOnline.AccesoDatos.Context; // Contiene el contexto de la base de datos.
using TiendaOnline.Dominio.Entidades; // Contiene las entidades utilizadas por el sistema.
using TiendaOnline.Dominio.InterfacesAD; // Contiene la interfaz de la unidad de trabajo.

namespace TiendaOnline.AccesoDatos.Implementaciones
{
    // Clase encargada de centralizar los repositorios y las operaciones de la base de datos.
    public class UnidadTrabajoEF : IUnidadTrabajoEF
    {
        // Sección donde se declaran los atributos de la clase.
        #region Atributos y variables

        // Contiene el contexto utilizado para acceder a la base de datos.
        private TiendaOnlineContext _Contexto { get; set; }

        // Guarda la transacción activa de la base de datos.
        private IDbContextTransaction? _transaction;

        // Guarda el repositorio de usuarios cuando es utilizado.
        private RepositorioAD<Usuario>? _TUsuario;

        // Guarda el repositorio de roles cuando es utilizado.
        private RepositorioAD<Rol>? _TRol;

        // Guarda el repositorio de productos cuando es utilizado.
        private RepositorioAD<Producto>? _TProducto;

        // Guarda el repositorio de categorías cuando es utilizado.
        private RepositorioAD<Categorium>? _TCategoria;

        // Guarda el repositorio de pedidos cuando es utilizado.
        private RepositorioAD<Pedido>? _TPedido;

        // Guarda el repositorio de detalles de pedidos cuando es utilizado.
        private RepositorioAD<DetallePedido>? _TDetallePedido;

        // Guarda el repositorio del inventario cuando es utilizado.
        private RepositorioAD<Inventario>? _TInventario;

        // Guarda el repositorio de movimientos de inventario cuando es utilizado.
        private RepositorioAD<MovimientoInventario>? _TMovimientoInventario;

        // Guarda el repositorio de descuentos cuando es utilizado.
        private RepositorioAD<Descuento>? _TDescuento;

        // Guarda el repositorio de proveedores cuando es utilizado.
        private RepositorioAD<Proveedor>? _TProveedor;

        // Guarda el repositorio de productos relacionados con proveedores.
        private RepositorioAD<ProductoProveedor>? _TProductoProveedor;

        // Guarda el repositorio de carritos de compra.
        private RepositorioAD<Carrito>? _TCarrito;

        // Guarda el repositorio de detalles de los carritos.
        private RepositorioAD<DetalleCarrito>? _TDetalleCarrito;

        // Guarda el repositorio de listas de deseos.
        private RepositorioAD<ListaDeseo>? _TListaDeseo;

        // Guarda el repositorio de detalles de las listas de deseos.
        private RepositorioAD<DetalleListaDeseo>? _TDetalleListaDeseo;

        // Guarda el repositorio de métodos de pago.
        private RepositorioAD<MetodoPago>? _TMetodoPago;

        // Guarda el repositorio de pagos.
        private RepositorioAD<Pago>? _TPago;

        // Guarda el repositorio de estados de pago.
        private RepositorioAD<EstadoPago>? _TEstadoPago;

        // Guarda el repositorio de estados de pedido.
        private RepositorioAD<EstadoPedido>? _TEstadoPedido;

        // Guarda el repositorio de envíos.
        private RepositorioAD<Envio>? _TEnvio;

        // Guarda el repositorio de direcciones de usuarios.
        private RepositorioAD<DireccionUsuario>? _TDireccionUsuario;

        // Guarda el repositorio de facturas.
        private RepositorioAD<Factura>? _TFactura;

        // Guarda el repositorio de evaluaciones de productos.
        private RepositorioAD<EvaluacionProducto>? _TEvaluacionProducto;

        // Guarda el repositorio de impuestos.
        private RepositorioAD<Impuesto>? _TImpuesto;

        // Guarda el repositorio de familias de productos.
        private RepositorioAD<FamiliaProducto>? _TFamiliaProducto;

        // Guarda el repositorio de compras realizadas a proveedores.
        private RepositorioAD<CompraProveedor>? _TCompraProveedor;

        // Guarda el repositorio de detalles de compras a proveedores.
        private RepositorioAD<DetalleCompraProveedor>? _TDetalleCompraProveedor;

        // Guarda el repositorio de proformas.
        private RepositorioAD<Proforma>? _TProforma;

        // Guarda el repositorio de detalles de las proformas.
        private RepositorioAD<DetalleProforma>? _TDetalleProforma;

        // Guarda el repositorio de notificaciones.
        private RepositorioAD<Notificacion>? _TNotificacion;

        // Guarda el repositorio del historial de accesos.
        private RepositorioAD<HistorialAcceso>? _THistorialAcceso;

        // Guarda el repositorio de la bitácora del sistema.
        private RepositorioAD<BitacoraSistema>? _TBitacoraSistema;

        #endregion

        // Sección donde se encuentra el constructor de la clase.
        #region Constructor

        // Constructor que recibe el contexto mediante inyección de dependencias.
        public UnidadTrabajoEF(TiendaOnlineContext contexto)
        {
            // Guarda el contexto recibido para utilizarlo en los repositorios.
            _Contexto = contexto;
        }

        #endregion

        // Sección donde se encuentran las propiedades de los repositorios.
        #region Repositorios

        // Permite acceder al repositorio de usuarios.
        public IRepositorioAD<Usuario> TUsuario
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TUsuario == null)
                {
                    // Crea el repositorio de usuarios utilizando el contexto actual.
                    _TUsuario = new RepositorioAD<Usuario>(_Contexto);
                }

                // Devuelve el repositorio de usuarios.
                return _TUsuario;
            }
        }

        // Permite acceder al repositorio de roles.
        public IRepositorioAD<Rol> TRol
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TRol == null)
                {
                    // Crea el repositorio de roles utilizando el contexto actual.
                    _TRol = new RepositorioAD<Rol>(_Contexto);
                }

                // Devuelve el repositorio de roles.
                return _TRol;
            }
        }

        // Permite acceder al repositorio de productos.
        public IRepositorioAD<Producto> TProducto
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TProducto == null)
                {
                    // Crea el repositorio de productos utilizando el contexto actual.
                    _TProducto = new RepositorioAD<Producto>(_Contexto);
                }

                // Devuelve el repositorio de productos.
                return _TProducto;
            }
        }

        // Permite acceder al repositorio de categorías.
        public IRepositorioAD<Categorium> TCategoria
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TCategoria == null)
                {
                    // Crea el repositorio de categorías utilizando el contexto actual.
                    _TCategoria = new RepositorioAD<Categorium>(_Contexto);
                }

                // Devuelve el repositorio de categorías.
                return _TCategoria;
            }
        }

        // Permite acceder al repositorio de pedidos.
        public IRepositorioAD<Pedido> TPedido
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TPedido == null)
                {
                    // Crea el repositorio de pedidos utilizando el contexto actual.
                    _TPedido = new RepositorioAD<Pedido>(_Contexto);
                }

                // Devuelve el repositorio de pedidos.
                return _TPedido;
            }
        }

        // Permite acceder al repositorio de detalles de pedidos.
        public IRepositorioAD<DetallePedido> TDetallePedido
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TDetallePedido == null)
                {
                    // Crea el repositorio de detalles de pedidos.
                    _TDetallePedido = new RepositorioAD<DetallePedido>(_Contexto);
                }

                // Devuelve el repositorio de detalles de pedidos.
                return _TDetallePedido;
            }
        }

        // Permite acceder al repositorio del inventario.
        public IRepositorioAD<Inventario> TInventario
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TInventario == null)
                {
                    // Crea el repositorio del inventario.
                    _TInventario = new RepositorioAD<Inventario>(_Contexto);
                }

                // Devuelve el repositorio del inventario.
                return _TInventario;
            }
        }

        // Permite acceder al repositorio de movimientos de inventario.
        public IRepositorioAD<MovimientoInventario> TMovimientoInventario
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TMovimientoInventario == null)
                {
                    // Crea el repositorio de movimientos de inventario.
                    _TMovimientoInventario =
                        new RepositorioAD<MovimientoInventario>(_Contexto);
                }

                // Devuelve el repositorio de movimientos.
                return _TMovimientoInventario;
            }
        }

        // Permite acceder al repositorio de descuentos.
        public IRepositorioAD<Descuento> TDescuento
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TDescuento == null)
                {
                    // Crea el repositorio de descuentos.
                    _TDescuento = new RepositorioAD<Descuento>(_Contexto);
                }

                // Devuelve el repositorio de descuentos.
                return _TDescuento;
            }
        }

        // Permite acceder al repositorio de proveedores.
        public IRepositorioAD<Proveedor> TProveedor
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TProveedor == null)
                {
                    // Crea el repositorio de proveedores.
                    _TProveedor = new RepositorioAD<Proveedor>(_Contexto);
                }

                // Devuelve el repositorio de proveedores.
                return _TProveedor;
            }
        }

        // Permite acceder al repositorio de relación entre productos y proveedores.
        public IRepositorioAD<ProductoProveedor> TProductoProveedor
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TProductoProveedor == null)
                {
                    // Crea el repositorio de productos y proveedores.
                    _TProductoProveedor =
                        new RepositorioAD<ProductoProveedor>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TProductoProveedor;
            }
        }

        // Permite acceder al repositorio de carritos.
        public IRepositorioAD<Carrito> TCarrito
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TCarrito == null)
                {
                    // Crea el repositorio de carritos.
                    _TCarrito = new RepositorioAD<Carrito>(_Contexto);
                }

                // Devuelve el repositorio de carritos.
                return _TCarrito;
            }
        }

        // Permite acceder al repositorio de detalles de carritos.
        public IRepositorioAD<DetalleCarrito> TDetalleCarrito
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TDetalleCarrito == null)
                {
                    // Crea el repositorio de detalles de carritos.
                    _TDetalleCarrito =
                        new RepositorioAD<DetalleCarrito>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TDetalleCarrito;
            }
        }

        // Permite acceder al repositorio de listas de deseos.
        public IRepositorioAD<ListaDeseo> TListaDeseo
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TListaDeseo == null)
                {
                    // Crea el repositorio de listas de deseos.
                    _TListaDeseo = new RepositorioAD<ListaDeseo>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TListaDeseo;
            }
        }

        // Permite acceder al repositorio de detalles de listas de deseos.
        public IRepositorioAD<DetalleListaDeseo> TDetalleListaDeseo
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TDetalleListaDeseo == null)
                {
                    // Crea el repositorio de detalles de listas de deseos.
                    _TDetalleListaDeseo =
                        new RepositorioAD<DetalleListaDeseo>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TDetalleListaDeseo;
            }
        }

        // Permite acceder al repositorio de métodos de pago.
        public IRepositorioAD<MetodoPago> TMetodoPago
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TMetodoPago == null)
                {
                    // Crea el repositorio de métodos de pago.
                    _TMetodoPago = new RepositorioAD<MetodoPago>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TMetodoPago;
            }
        }

        // Permite acceder al repositorio de pagos.
        public IRepositorioAD<Pago> TPago
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TPago == null)
                {
                    // Crea el repositorio de pagos.
                    _TPago = new RepositorioAD<Pago>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TPago;
            }
        }

        // Permite acceder al repositorio de estados de pago.
        public IRepositorioAD<EstadoPago> TEstadoPago
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TEstadoPago == null)
                {
                    // Crea el repositorio de estados de pago.
                    _TEstadoPago = new RepositorioAD<EstadoPago>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TEstadoPago;
            }
        }

        // Permite acceder al repositorio de estados de pedido.
        public IRepositorioAD<EstadoPedido> TEstadoPedido
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TEstadoPedido == null)
                {
                    // Crea el repositorio de estados de pedido.
                    _TEstadoPedido = new RepositorioAD<EstadoPedido>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TEstadoPedido;
            }
        }

        // Permite acceder al repositorio de envíos.
        public IRepositorioAD<Envio> TEnvio
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TEnvio == null)
                {
                    // Crea el repositorio de envíos.
                    _TEnvio = new RepositorioAD<Envio>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TEnvio;
            }
        }

        // Permite acceder al repositorio de direcciones de usuarios.
        public IRepositorioAD<DireccionUsuario> TDireccionUsuario
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TDireccionUsuario == null)
                {
                    // Crea el repositorio de direcciones.
                    _TDireccionUsuario =
                        new RepositorioAD<DireccionUsuario>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TDireccionUsuario;
            }
        }

        // Permite acceder al repositorio de facturas.
        public IRepositorioAD<Factura> TFactura
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TFactura == null)
                {
                    // Crea el repositorio de facturas.
                    _TFactura = new RepositorioAD<Factura>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TFactura;
            }
        }

        // Permite acceder al repositorio de evaluaciones de productos.
        public IRepositorioAD<EvaluacionProducto> TEvaluacionProducto
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TEvaluacionProducto == null)
                {
                    // Crea el repositorio de evaluaciones.
                    _TEvaluacionProducto =
                        new RepositorioAD<EvaluacionProducto>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TEvaluacionProducto;
            }
        }

        // Permite acceder al repositorio de impuestos.
        public IRepositorioAD<Impuesto> TImpuesto
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TImpuesto == null)
                {
                    // Crea el repositorio de impuestos.
                    _TImpuesto = new RepositorioAD<Impuesto>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TImpuesto;
            }
        }

        // Permite acceder al repositorio de familias de productos.
        public IRepositorioAD<FamiliaProducto> TFamiliaProducto
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TFamiliaProducto == null)
                {
                    // Crea el repositorio de familias de productos.
                    _TFamiliaProducto =
                        new RepositorioAD<FamiliaProducto>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TFamiliaProducto;
            }
        }

        // Permite acceder al repositorio de compras a proveedores.
        public IRepositorioAD<CompraProveedor> TCompraProveedor
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TCompraProveedor == null)
                {
                    // Crea el repositorio de compras a proveedores.
                    _TCompraProveedor =
                        new RepositorioAD<CompraProveedor>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TCompraProveedor;
            }
        }

        // Permite acceder al repositorio de detalles de compras a proveedores.
        public IRepositorioAD<DetalleCompraProveedor> TDetalleCompraProveedor
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TDetalleCompraProveedor == null)
                {
                    // Crea el repositorio de detalles de compras.
                    _TDetalleCompraProveedor =
                        new RepositorioAD<DetalleCompraProveedor>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TDetalleCompraProveedor;
            }
        }

        // Permite acceder al repositorio de proformas.
        public IRepositorioAD<Proforma> TProforma
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TProforma == null)
                {
                    // Crea el repositorio de proformas.
                    _TProforma = new RepositorioAD<Proforma>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TProforma;
            }
        }

        // Permite acceder al repositorio de detalles de proformas.
        public IRepositorioAD<DetalleProforma> TDetalleProforma
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TDetalleProforma == null)
                {
                    // Crea el repositorio de detalles de proformas.
                    _TDetalleProforma =
                        new RepositorioAD<DetalleProforma>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TDetalleProforma;
            }
        }

        // Permite acceder al repositorio de notificaciones.
        public IRepositorioAD<Notificacion> TNotificacion
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TNotificacion == null)
                {
                    // Crea el repositorio de notificaciones.
                    _TNotificacion =
                        new RepositorioAD<Notificacion>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TNotificacion;
            }
        }

        // Permite acceder al repositorio del historial de accesos.
        public IRepositorioAD<HistorialAcceso> THistorialAcceso
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_THistorialAcceso == null)
                {
                    // Crea el repositorio del historial de accesos.
                    _THistorialAcceso =
                        new RepositorioAD<HistorialAcceso>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _THistorialAcceso;
            }
        }

        // Permite acceder al repositorio de la bitácora del sistema.
        public IRepositorioAD<BitacoraSistema> TBitacoraSistema
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TBitacoraSistema == null)
                {
                    // Crea el repositorio de la bitácora.
                    _TBitacoraSistema =
                        new RepositorioAD<BitacoraSistema>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TBitacoraSistema;
            }
        }

        #endregion

        // Sección donde se manejan los cambios y las transacciones.
        #region Cambios y transacciones

        // Guarda todos los cambios pendientes en la base de datos.
        public int Completar()
        {
            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Guarda los cambios y devuelve la cantidad de registros afectados.
                return _Contexto.SaveChanges();
            }
            // Captura cualquier error ocurrido durante el guardado.
            catch
            {
                // Vuelve a lanzar el error para que pueda ser controlado externamente.
                throw;
            }
        }

        // Guarda los cambios y confirma la transacción actual.
        public void CompletarTran()
        {
            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Guarda los cambios pendientes en la base de datos.
                _Contexto.SaveChanges();

                // Comprueba si existe una transacción activa.
                if (_transaction != null)
                {
                    // Confirma definitivamente los cambios realizados.
                    _transaction.Commit();
                }
            }
            // Captura cualquier error ocurrido durante la operación.
            catch
            {
                // Comprueba si existe una transacción activa.
                if (_transaction != null)
                {
                    // Revierte los cambios realizados dentro de la transacción.
                    _transaction.Rollback();
                }

                // Vuelve a lanzar el error para que pueda ser controlado externamente.
                throw;
            }
        }

        // Inicia una nueva transacción en la base de datos.
        public void EmpezarTransaccion()
        {
            // Crea una transacción utilizando el contexto actual.
            _transaction = _Contexto.Database.BeginTransaction();
        }

        // Revierte los cambios realizados dentro de la transacción.
        public void Rollback()
        {
            // Comprueba si existe una transacción activa.
            if (_transaction != null)
            {
                // Cancela la transacción y devuelve los datos a su estado anterior.
                _transaction.Rollback();
            }
        }

        // Cierra la conexión con la base de datos.
        public void CerrarConexion()
        {
            // Cierra la conexión utilizada por el contexto.
            _Contexto.Database.CloseConnection();
        }

        // Libera los recursos utilizados por la unidad de trabajo.
        public void Dispose()
        {
            // Comprueba si existe una transacción activa.
            if (_transaction != null)
            {
                // Libera los recursos utilizados por la transacción.
                _transaction.Dispose();
            }

            // Libera los recursos utilizados por el contexto de Entity Framework.
            _Contexto.Dispose();
        }

        #endregion
    }
}