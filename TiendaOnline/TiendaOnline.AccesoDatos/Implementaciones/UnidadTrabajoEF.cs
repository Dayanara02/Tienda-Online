using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Dominio.InterfacesAD;

namespace TiendaOnline.AccesoDatos.Implementaciones
{
    // Esta clase implementa la interfaz IUnidadTrabajoEF.
    // Su función principal es centralizar el acceso a los diferentes repositorios
    // y permitir que todos trabajen utilizando el mismo contexto de Entity Framework.
    public class UnidadTrabajoEF : IUnidadTrabajoEF
    {
        // Este region sirve solamente para ordenar visualmente el código.
        // Dentro se colocan las variables que utiliza la clase.
        #region Atributos y variables

        // Guarda el contexto principal de Entity Framework.
        // Este contexto representa la conexión y las tablas de la base de datos TiendaOnline.
        private TiendaOnlineContext _Contexto { get; set; }

        // Guarda una transacción activa de la base de datos.
        // Sirve para poder confirmar varios cambios juntos o devolverlos si algo falla.
        private IDbContextTransaction? _transaction;

        // Cada una de estas variables almacenará un repositorio específico.
        // Al principio están vacías y se crean solamente cuando se necesitan.

        // Repositorio utilizado para trabajar con los usuarios.
        private RepositorioAD<Usuario>? _TUsuario;

        // Repositorio utilizado para trabajar con los roles.
        private RepositorioAD<Rol>? _TRol;

        // Repositorio utilizado para trabajar con los productos.
        private RepositorioAD<Producto>? _TProducto;

        // Repositorio utilizado para trabajar con las categorías.
        private RepositorioAD<Categorium>? _TCategoria;

        // Repositorio utilizado para trabajar con los pedidos.
        private RepositorioAD<Pedido>? _TPedido;

        // Repositorio utilizado para trabajar con los detalles de cada pedido.
        private RepositorioAD<DetallePedido>? _TDetallePedido;

        // Repositorio utilizado para trabajar con el inventario.
        private RepositorioAD<Inventario>? _TInventario;

        // Repositorio utilizado para registrar entradas, salidas
        // y demás movimientos realizados en el inventario.
        private RepositorioAD<MovimientoInventario>? _TMovimientoInventario;

        // Repositorio utilizado para trabajar con descuentos.
        private RepositorioAD<Descuento>? _TDescuento;

        // Repositorio utilizado para trabajar con proveedores.
        private RepositorioAD<Proveedor>? _TProveedor;

        // Repositorio utilizado para trabajar con la relación
        // entre los productos y sus proveedores.
        private RepositorioAD<ProductoProveedor>? _TProductoProveedor;

        // Repositorio utilizado para trabajar con los carritos de compra.
        private RepositorioAD<Carrito>? _TCarrito;

        // Repositorio utilizado para trabajar con los productos
        // agregados dentro de cada carrito.
        private RepositorioAD<DetalleCarrito>? _TDetalleCarrito;

        // Repositorio utilizado para trabajar con las listas de deseos.
        private RepositorioAD<ListaDeseo>? _TListaDeseo;

        // Repositorio utilizado para trabajar con los productos
        // guardados dentro de una lista de deseos.
        private RepositorioAD<DetalleListaDeseo>? _TDetalleListaDeseo;

        // Repositorio utilizado para trabajar con los métodos de pago.
        private RepositorioAD<MetodoPago>? _TMetodoPago;

        // Repositorio utilizado para trabajar con los pagos realizados.
        private RepositorioAD<Pago>? _TPago;

        // Repositorio utilizado para trabajar con los estados de los pagos.
        private RepositorioAD<EstadoPago>? _TEstadoPago;

        // Repositorio utilizado para trabajar con los estados de los pedidos.
        private RepositorioAD<EstadoPedido>? _TEstadoPedido;

        // Repositorio utilizado para trabajar con los envíos.
        private RepositorioAD<Envio>? _TEnvio;

        // Repositorio utilizado para trabajar con las direcciones de los usuarios.
        private RepositorioAD<DireccionUsuario>? _TDireccionUsuario;

        // Repositorio utilizado para trabajar con las facturas.
        private RepositorioAD<Factura>? _TFactura;

        // Repositorio utilizado para trabajar con las evaluaciones
        // o calificaciones realizadas a los productos.
        private RepositorioAD<EvaluacionProducto>? _TEvaluacionProducto;

        // Repositorio utilizado para trabajar con los impuestos.
        private RepositorioAD<Impuesto>? _TImpuesto;

        // Repositorio utilizado para trabajar con las familias de productos.
        private RepositorioAD<FamiliaProducto>? _TFamiliaProducto;

        // Repositorio utilizado para trabajar con las compras hechas a proveedores.
        private RepositorioAD<CompraProveedor>? _TCompraProveedor;

        // Repositorio utilizado para trabajar con los productos
        // incluidos dentro de cada compra a proveedor.
        private RepositorioAD<DetalleCompraProveedor>? _TDetalleCompraProveedor;

        // Repositorio utilizado para trabajar con las proformas.
        private RepositorioAD<Proforma>? _TProforma;

        // Repositorio utilizado para trabajar con el detalle de las proformas.
        private RepositorioAD<DetalleProforma>? _TDetalleProforma;

        // Repositorio utilizado para trabajar con las notificaciones.
        private RepositorioAD<Notificacion>? _TNotificacion;

        // Repositorio utilizado para guardar y consultar
        // los accesos realizados por los usuarios.
        private RepositorioAD<HistorialAcceso>? _THistorialAcceso;

        // Repositorio utilizado para trabajar con la bitácora del sistema.
        private RepositorioAD<BitacoraSistema>? _TBitacoraSistema;

        // Finaliza la sección visual de atributos y variables.
        #endregion


        // Esta sección contiene el constructor de la clase.
        #region Constructor

        // El constructor recibe TiendaOnlineContext mediante inyección de dependencias.
        // Esto permite que UnidadTrabajoEF utilice el contexto que ya fue configurado
        // con la conexión hacia la base de datos.
        public UnidadTrabajoEF(TiendaOnlineContext contexto)
        {
            // Guarda el contexto recibido dentro de la variable _Contexto.
            // Así se puede utilizar posteriormente en todos los repositorios.
            _Contexto = contexto;
        }

        // Finaliza la sección visual del constructor.
        #endregion


        // Esta sección contiene las propiedades que permiten obtener cada repositorio.
        #region Repositorios


        // Esta propiedad permite acceder al repositorio de usuarios.
        public IRepositorioAD<Usuario> TUsuario
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TUsuario == null)
                {
                    // Crea un repositorio para Usuario utilizando el mismo contexto.
                    _TUsuario = new RepositorioAD<Usuario>(_Contexto);
                }

                // Devuelve el repositorio para poder realizar operaciones con usuarios.
                return _TUsuario;
            }
        }


        // Esta propiedad permite acceder al repositorio de roles.
        public IRepositorioAD<Rol> TRol
        {
            get
            {
                // Solo crea el repositorio si todavía no existe.
                if (_TRol == null)
                {
                    // Crea un repositorio genérico preparado para trabajar con Rol.
                    _TRol = new RepositorioAD<Rol>(_Contexto);
                }

                // Devuelve el repositorio de roles.
                return _TRol;
            }
        }


        // Esta propiedad permite acceder al repositorio de productos.
        public IRepositorioAD<Producto> TProducto
        {
            get
            {
                // Revisa si todavía no existe una instancia del repositorio.
                if (_TProducto == null)
                {
                    // Crea el repositorio de productos utilizando el contexto actual.
                    _TProducto = new RepositorioAD<Producto>(_Contexto);
                }

                // Devuelve el repositorio para trabajar con productos.
                return _TProducto;
            }
        }


        // Esta propiedad permite acceder al repositorio de categorías.
        public IRepositorioAD<Categorium> TCategoria
        {
            get
            {
                // Verifica si el repositorio todavía no ha sido creado.
                if (_TCategoria == null)
                {
                    // Crea un repositorio preparado para trabajar con Categorium.
                    _TCategoria = new RepositorioAD<Categorium>(_Contexto);
                }

                // Devuelve el repositorio de categorías.
                return _TCategoria;
            }
        }


        // Esta propiedad permite acceder al repositorio de pedidos.
        public IRepositorioAD<Pedido> TPedido
        {
            get
            {
                // Comprueba si el repositorio de pedidos todavía está vacío.
                if (_TPedido == null)
                {
                    // Crea un repositorio utilizando la entidad Pedido.
                    _TPedido = new RepositorioAD<Pedido>(_Contexto);
                }

                // Devuelve el repositorio de pedidos.
                return _TPedido;
            }
        }


        // Esta propiedad permite acceder a los detalles de los pedidos.
        public IRepositorioAD<DetallePedido> TDetallePedido
        {
            get
            {
                // Comprueba si el repositorio todavía no existe.
                if (_TDetallePedido == null)
                {
                    // Crea un repositorio para trabajar con DetallePedido.
                    _TDetallePedido =
                        new RepositorioAD<DetallePedido>(_Contexto);
                }

                // Devuelve el repositorio de detalles de pedidos.
                return _TDetallePedido;
            }
        }


        // Esta propiedad permite acceder al repositorio del inventario.
        public IRepositorioAD<Inventario> TInventario
        {
            get
            {
                // Verifica si todavía no se ha creado el repositorio.
                if (_TInventario == null)
                {
                    // Crea el repositorio para trabajar con el inventario.
                    _TInventario =
                        new RepositorioAD<Inventario>(_Contexto);
                }

                // Devuelve el repositorio del inventario.
                return _TInventario;
            }
        }


        // Esta propiedad permite trabajar con los movimientos del inventario.
        public IRepositorioAD<MovimientoInventario> TMovimientoInventario
        {
            get
            {
                // Comprueba si todavía no existe un repositorio para los movimientos.
                if (_TMovimientoInventario == null)
                {
                    // Crea el repositorio utilizando la entidad MovimientoInventario.
                    _TMovimientoInventario =
                        new RepositorioAD<MovimientoInventario>(_Contexto);
                }

                // Devuelve el repositorio de movimientos de inventario.
                return _TMovimientoInventario;
            }
        }


        // Esta propiedad permite acceder a los descuentos.
        public IRepositorioAD<Descuento> TDescuento
        {
            get
            {
                // Verifica si el repositorio todavía no existe.
                if (_TDescuento == null)
                {
                    // Crea el repositorio para trabajar con los descuentos.
                    _TDescuento =
                        new RepositorioAD<Descuento>(_Contexto);
                }

                // Devuelve el repositorio de descuentos.
                return _TDescuento;
            }
        }


        // Esta propiedad permite acceder a los proveedores.
        public IRepositorioAD<Proveedor> TProveedor
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TProveedor == null)
                {
                    // Crea un repositorio para trabajar con proveedores.
                    _TProveedor =
                        new RepositorioAD<Proveedor>(_Contexto);
                }

                // Devuelve el repositorio de proveedores.
                return _TProveedor;
            }
        }


        // Esta propiedad maneja la relación entre productos y proveedores.
        public IRepositorioAD<ProductoProveedor> TProductoProveedor
        {
            get
            {
                // Verifica si todavía no existe un repositorio.
                if (_TProductoProveedor == null)
                {
                    // Crea el repositorio usando la entidad ProductoProveedor.
                    _TProductoProveedor =
                        new RepositorioAD<ProductoProveedor>(_Contexto);
                }

                // Devuelve el repositorio de la relación producto-proveedor.
                return _TProductoProveedor;
            }
        }


        // Esta propiedad permite acceder a los carritos de compra.
        public IRepositorioAD<Carrito> TCarrito
        {
            get
            {
                // Comprueba si todavía no se ha creado el repositorio.
                if (_TCarrito == null)
                {
                    // Crea un repositorio para trabajar con los carritos.
                    _TCarrito =
                        new RepositorioAD<Carrito>(_Contexto);
                }

                // Devuelve el repositorio de carritos.
                return _TCarrito;
            }
        }


        // Esta propiedad permite acceder a los detalles del carrito.
        public IRepositorioAD<DetalleCarrito> TDetalleCarrito
        {
            get
            {
                // Verifica si todavía no existe un repositorio.
                if (_TDetalleCarrito == null)
                {
                    // Crea el repositorio para trabajar con DetalleCarrito.
                    _TDetalleCarrito =
                        new RepositorioAD<DetalleCarrito>(_Contexto);
                }

                // Devuelve el repositorio de detalles del carrito.
                return _TDetalleCarrito;
            }
        }


        // Esta propiedad permite trabajar con las listas de deseos.
        public IRepositorioAD<ListaDeseo> TListaDeseo
        {
            get
            {
                // Revisa si el repositorio todavía no ha sido creado.
                if (_TListaDeseo == null)
                {
                    // Crea el repositorio de listas de deseos.
                    _TListaDeseo =
                        new RepositorioAD<ListaDeseo>(_Contexto);
                }

                // Devuelve el repositorio de listas de deseos.
                return _TListaDeseo;
            }
        }


        // Esta propiedad permite acceder a los productos guardados en listas de deseos.
        public IRepositorioAD<DetalleListaDeseo> TDetalleListaDeseo
        {
            get
            {
                // Comprueba si el repositorio todavía no existe.
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


        // Esta propiedad permite acceder a los métodos de pago.
        public IRepositorioAD<MetodoPago> TMetodoPago
        {
            get
            {
                // Comprueba si todavía no existe el repositorio.
                if (_TMetodoPago == null)
                {
                    // Crea un repositorio para trabajar con MetodoPago.
                    _TMetodoPago =
                        new RepositorioAD<MetodoPago>(_Contexto);
                }

                // Devuelve el repositorio de métodos de pago.
                return _TMetodoPago;
            }
        }


        // Esta propiedad permite acceder a los pagos realizados.
        public IRepositorioAD<Pago> TPago
        {
            get
            {
                // Comprueba si todavía no se ha creado el repositorio.
                if (_TPago == null)
                {
                    // Crea un repositorio para trabajar con Pago.
                    _TPago =
                        new RepositorioAD<Pago>(_Contexto);
                }

                // Devuelve el repositorio de pagos.
                return _TPago;
            }
        }


        // Esta propiedad permite trabajar con los estados de pago.
        public IRepositorioAD<EstadoPago> TEstadoPago
        {
            get
            {
                // Verifica si el repositorio todavía está vacío.
                if (_TEstadoPago == null)
                {
                    // Crea un repositorio para trabajar con EstadoPago.
                    _TEstadoPago =
                        new RepositorioAD<EstadoPago>(_Contexto);
                }

                // Devuelve el repositorio de estados de pago.
                return _TEstadoPago;
            }
        }


        // Esta propiedad permite trabajar con los estados de los pedidos.
        public IRepositorioAD<EstadoPedido> TEstadoPedido
        {
            get
            {
                // Comprueba si el repositorio todavía no existe.
                if (_TEstadoPedido == null)
                {
                    // Crea un repositorio para trabajar con EstadoPedido.
                    _TEstadoPedido =
                        new RepositorioAD<EstadoPedido>(_Contexto);
                }

                // Devuelve el repositorio de estados de pedido.
                return _TEstadoPedido;
            }
        }


        // Esta propiedad permite trabajar con los envíos.
        public IRepositorioAD<Envio> TEnvio
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TEnvio == null)
                {
                    // Crea un repositorio para trabajar con la entidad Envio.
                    _TEnvio =
                        new RepositorioAD<Envio>(_Contexto);
                }

                // Devuelve el repositorio de envíos.
                return _TEnvio;
            }
        }


        // Esta propiedad permite trabajar con las direcciones de los usuarios.
        public IRepositorioAD<DireccionUsuario> TDireccionUsuario
        {
            get
            {
                // Verifica si el repositorio todavía no existe.
                if (_TDireccionUsuario == null)
                {
                    // Crea el repositorio para trabajar con DireccionUsuario.
                    _TDireccionUsuario =
                        new RepositorioAD<DireccionUsuario>(_Contexto);
                }

                // Devuelve el repositorio de direcciones.
                return _TDireccionUsuario;
            }
        }


        // Esta propiedad permite acceder a las facturas.
        public IRepositorioAD<Factura> TFactura
        {
            get
            {
                // Comprueba si todavía no se ha creado el repositorio.
                if (_TFactura == null)
                {
                    // Crea un repositorio preparado para trabajar con Factura.
                    _TFactura =
                        new RepositorioAD<Factura>(_Contexto);
                }

                // Devuelve el repositorio de facturas.
                return _TFactura;
            }
        }


        // Esta propiedad permite trabajar con evaluaciones de productos.
        public IRepositorioAD<EvaluacionProducto> TEvaluacionProducto
        {
            get
            {
                // Comprueba si todavía no existe el repositorio.
                if (_TEvaluacionProducto == null)
                {
                    // Crea el repositorio para trabajar con EvaluacionProducto.
                    _TEvaluacionProducto =
                        new RepositorioAD<EvaluacionProducto>(_Contexto);
                }

                // Devuelve el repositorio de evaluaciones.
                return _TEvaluacionProducto;
            }
        }


        // Esta propiedad permite trabajar con los impuestos.
        public IRepositorioAD<Impuesto> TImpuesto
        {
            get
            {
                // Revisa si todavía no existe un repositorio.
                if (_TImpuesto == null)
                {
                    // Crea el repositorio para trabajar con Impuesto.
                    _TImpuesto =
                        new RepositorioAD<Impuesto>(_Contexto);
                }

                // Devuelve el repositorio de impuestos.
                return _TImpuesto;
            }
        }


        // Esta propiedad permite trabajar con las familias de productos.
        public IRepositorioAD<FamiliaProducto> TFamiliaProducto
        {
            get
            {
                // Comprueba si el repositorio todavía no ha sido creado.
                if (_TFamiliaProducto == null)
                {
                    // Crea el repositorio para trabajar con FamiliaProducto.
                    _TFamiliaProducto =
                        new RepositorioAD<FamiliaProducto>(_Contexto);
                }

                // Devuelve el repositorio de familias de productos.
                return _TFamiliaProducto;
            }
        }


        // Esta propiedad permite trabajar con las compras a proveedores.
        public IRepositorioAD<CompraProveedor> TCompraProveedor
        {
            get
            {
                // Comprueba si todavía no existe un repositorio.
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


        // Esta propiedad permite trabajar con los detalles
        // de las compras realizadas a los proveedores.
        public IRepositorioAD<DetalleCompraProveedor> TDetalleCompraProveedor
        {
            get
            {
                // Comprueba si todavía no ha sido creado el repositorio.
                if (_TDetalleCompraProveedor == null)
                {
                    // Crea un repositorio usando DetalleCompraProveedor.
                    _TDetalleCompraProveedor =
                        new RepositorioAD<DetalleCompraProveedor>(_Contexto);
                }

                // Devuelve el repositorio de detalles de compras.
                return _TDetalleCompraProveedor;
            }
        }


        // Esta propiedad permite trabajar con las proformas.
        public IRepositorioAD<Proforma> TProforma
        {
            get
            {
                // Verifica si todavía no existe un repositorio.
                if (_TProforma == null)
                {
                    // Crea un repositorio para la entidad Proforma.
                    _TProforma =
                        new RepositorioAD<Proforma>(_Contexto);
                }

                // Devuelve el repositorio de proformas.
                return _TProforma;
            }
        }


        // Esta propiedad permite trabajar con los detalles de las proformas.
        public IRepositorioAD<DetalleProforma> TDetalleProforma
        {
            get
            {
                // Comprueba si todavía no se ha creado el repositorio.
                if (_TDetalleProforma == null)
                {
                    // Crea el repositorio para trabajar con DetalleProforma.
                    _TDetalleProforma =
                        new RepositorioAD<DetalleProforma>(_Contexto);
                }

                // Devuelve el repositorio correspondiente.
                return _TDetalleProforma;
            }
        }


        // Esta propiedad permite trabajar con las notificaciones.
        public IRepositorioAD<Notificacion> TNotificacion
        {
            get
            {
                // Verifica si todavía no existe un repositorio.
                if (_TNotificacion == null)
                {
                    // Crea el repositorio para trabajar con Notificacion.
                    _TNotificacion =
                        new RepositorioAD<Notificacion>(_Contexto);
                }

                // Devuelve el repositorio de notificaciones.
                return _TNotificacion;
            }
        }


        // Esta propiedad permite trabajar con el historial de accesos.
        public IRepositorioAD<HistorialAcceso> THistorialAcceso
        {
            get
            {
                // Comprueba si todavía no se ha creado el repositorio.
                if (_THistorialAcceso == null)
                {
                    // Crea un repositorio para la entidad HistorialAcceso.
                    _THistorialAcceso =
                        new RepositorioAD<HistorialAcceso>(_Contexto);
                }

                // Devuelve el repositorio del historial de accesos.
                return _THistorialAcceso;
            }
        }


        // Esta propiedad permite trabajar con la bitácora del sistema.
        public IRepositorioAD<BitacoraSistema> TBitacoraSistema
        {
            get
            {
                // Comprueba si todavía no existe el repositorio.
                if (_TBitacoraSistema == null)
                {
                    // Crea un repositorio para trabajar con BitacoraSistema.
                    _TBitacoraSistema =
                        new RepositorioAD<BitacoraSistema>(_Contexto);
                }

                // Devuelve el repositorio de la bitácora.
                return _TBitacoraSistema;
            }
        }


        // Finaliza la sección visual donde están todos los repositorios.
        #endregion


        // Esta sección contiene los métodos utilizados para guardar cambios
        // y controlar transacciones de la base de datos.
        #region Cambios y transacciones


        // Este método guarda todos los cambios pendientes del contexto.
        public int Completar()
        {
            try
            {
                // SaveChanges ejecuta en la base de datos todos los cambios pendientes,
                // como insertar, modificar o eliminar registros.
                // Además devuelve la cantidad de registros afectados.
                return _Contexto.SaveChanges();
            }
            catch
            {
                // Si ocurre algún error al guardar,
                // se vuelve a enviar el error a la capa que llamó este método.
                throw;
            }
        }


        // Este método guarda los cambios y confirma una transacción.
        // Se utiliza cuando varias operaciones deben completarse correctamente juntas.
        public void CompletarTran()
        {
            try
            {
                // Guarda primero todos los cambios pendientes en el contexto.
                _Contexto.SaveChanges();

                // Comprueba que exista una transacción activa.
                if (_transaction != null)
                {
                    // Commit confirma de forma definitiva todos los cambios
                    // realizados dentro de la transacción.
                    _transaction.Commit();
                }
            }
            catch
            {
                // Si ocurre un error, verifica si existe una transacción.
                if (_transaction != null)
                {
                    // Rollback devuelve la base de datos al estado anterior
                    // al inicio de la transacción.
                    _transaction.Rollback();
                }

                // Vuelve a lanzar el error para que pueda ser controlado
                // desde la lógica de negocio o la API.
                throw;
            }
        }


        // Este método inicia una nueva transacción.
        public void EmpezarTransaccion()
        {
            // BeginTransaction indica a Entity Framework que las siguientes operaciones
            // deben formar parte de una misma transacción.
            _transaction = _Contexto.Database.BeginTransaction();
        }


        // Este método cancela manualmente la transacción actual.
        public void Rollback()
        {
            // Comprueba que exista una transacción antes de intentar cancelarla.
            if (_transaction != null)
            {
                // Revierte todos los cambios realizados dentro de esa transacción.
                _transaction.Rollback();
            }
        }


        // Este método cierra la conexión utilizada por Entity Framework.
        public void CerrarConexion()
        {
            // CloseConnection cierra la conexión con SQL Server
            // cuando ya no es necesario mantenerla abierta.
            _Contexto.Database.CloseConnection();
        }


        // Dispose se utiliza para liberar recursos que ya no se necesitan.
        // Existe porque IUnidadTrabajoEF hereda de IDisposable.
        public void Dispose()
        {
            // Comprueba si existe una transacción.
            if (_transaction != null)
            {
                // Libera los recursos utilizados por la transacción.
                _transaction.Dispose();
            }

            // Libera los recursos utilizados por TiendaOnlineContext
            // cuando la unidad de trabajo deja de utilizarse.
            _Contexto.Dispose();
        }


        // Finaliza la sección visual de cambios y transacciones.
        #endregion
    }
}