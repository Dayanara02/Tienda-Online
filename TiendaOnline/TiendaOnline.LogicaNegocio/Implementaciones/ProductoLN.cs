using Microsoft.Extensions.Logging;
using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Dominio.InterfacesAD;
using TiendaOnline.Dominio.InterfacesLN;
using TiendaOnline.Utilidades;

namespace TiendaOnline.LogicaNegocio.Implementaciones
{
    // Esta clase contiene las reglas de negocio relacionadas con los productos.
    // Implementa IProductoLN para asegurar que tenga todas las operaciones
    // definidas para la lógica de productos.
    public class ProductoLN : IProductoLN
    {
        // Guarda la unidad de trabajo.
        // Por medio de ella podemos acceder al repositorio de productos
        // sin trabajar directamente con el contexto de la base de datos.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores o información importante
        // que ocurra dentro de la lógica de productos.
        private readonly ILogger<ProductoLN> _logger;


        // El constructor recibe las dependencias que necesita esta clase.
        // Estas dependencias serán entregadas mediante inyección de dependencias.
        public ProductoLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<ProductoLN> logger)
        {
            // Guarda la unidad de trabajo recibida para utilizar sus repositorios.
            _unidadDeTrabajo = unidadDeTrabajo;

            // Guarda el logger para poder registrar errores.
            _logger = logger;
        }


        // Este método se encarga de registrar un nuevo producto.
        public async Task<Respuesta<Producto>> InsertarAsync(Producto datos)
        {
            // Crea el objeto que devolverá el producto registrado
            // o el mensaje de error si ocurre algún problema.
            var resultado = new Respuesta<Producto>();

            try
            {
                // Busca si ya existe otro producto con el mismo código.
                // El código debe identificar de forma única al producto.
                var productoPorCodigo =
                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                        x => x.Codigo == datos.Codigo);

                // Si encuentra un producto, significa que el código ya está registrado.
                if (productoPorCodigo.Data != null)
                {
                    // Guarda el producto que ya existe para devolverlo como referencia.
                    resultado.Data = productoPorCodigo.Data;

                    // Informa por qué no se puede registrar el producto.
                    resultado.Error =
                        "Ya existe un producto registrado con ese código.";

                    // Termina el método sin realizar la inserción.
                    return resultado;
                }


                // Busca también si existe un producto con el mismo nombre.
                var productoPorNombre =
                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                        x => x.Nombre == datos.Nombre);

                // Evita registrar productos repetidos con exactamente el mismo nombre.
                if (productoPorNombre.Data != null)
                {
                    resultado.Data = productoPorNombre.Data;

                    resultado.Error =
                        "Ya existe un producto registrado con ese nombre.";

                    return resultado;
                }


                // Verifica que el precio del producto sea válido.
                if (datos.Precio <= 0)
                {
                    resultado.Error =
                        "El precio del producto debe ser mayor que cero.";

                    return resultado;
                }


                // Verifica que el costo no sea negativo.
                if (datos.Costo < 0)
                {
                    resultado.Error =
                        "El costo del producto no puede ser negativo.";

                    return resultado;
                }


                // Verifica que el stock mínimo tenga un valor válido.
                if (datos.StockMinimo < 0)
                {
                    resultado.Error =
                        "El stock mínimo no puede ser negativo.";

                    return resultado;
                }


                // Guarda automáticamente la fecha en la que se registra el producto.
                datos.FechaRegistro = DateTime.Now;


                // Envía el producto al repositorio para insertarlo en la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProducto.InsertarAsync(datos);


                // Verifica si el repositorio devolvió algún error.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda en la respuesta el producto que fue registrado.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra técnicamente el error para poder revisarlo
                // desde los registros de la aplicación.
                _logger.LogError(
                    ex,
                    "Error al insertar el producto {Nombre}",
                    datos.Nombre);

                // Guarda un mensaje que podrá ser enviado a la capa superior.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado final de la operación.
            return resultado;
        }


        // Este método obtiene todos los productos registrados.
        public async Task<Respuesta<IEnumerable<Producto>>> ListarAsync()
        {
            // Crea una respuesta preparada para devolver varios productos.
            var resultado =
                new Respuesta<IEnumerable<Producto>>();

            try
            {
                // Solicita al repositorio todos los registros de productos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProducto.ListarAsync();

                // Si el repositorio tuvo un error, se copia en la respuesta.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }

                // Guarda la lista obtenida para devolverla a la capa que llamó el método.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido al consultar los productos.
                _logger.LogError(
                    ex,
                    "Error al listar los productos.");

                // Guarda el mensaje del error dentro de la respuesta.
                resultado.Error = ex.Message;
            }

            return resultado;
        }


        // Este método modifica los datos de un producto existente.
        public async Task<Respuesta<Producto>> ModificarAsync(Producto datos)
        {
            // Crea la respuesta que devolverá el resultado de la modificación.
            var resultado = new Respuesta<Producto>();

            try
            {
                // Busca el producto en la base de datos utilizando su identificador.
                var productoActual =
                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                        x => x.IdProducto == datos.IdProducto);

                // Si no existe, no se puede realizar ninguna modificación.
                if (productoActual.Data == null)
                {
                    resultado.Error =
                        "No existe el producto que desea modificar.";

                    return resultado;
                }


                // Comprueba que el precio nuevo siga siendo válido.
                if (datos.Precio <= 0)
                {
                    resultado.Error =
                        "El precio del producto debe ser mayor que cero.";

                    return resultado;
                }


                // Comprueba que el costo no tenga un valor negativo.
                if (datos.Costo < 0)
                {
                    resultado.Error =
                        "El costo del producto no puede ser negativo.";

                    return resultado;
                }


                // Comprueba que el stock mínimo tampoco sea negativo.
                if (datos.StockMinimo < 0)
                {
                    resultado.Error =
                        "El stock mínimo no puede ser negativo.";

                    return resultado;
                }


                // Actualiza la categoría del producto.
                productoActual.Data.IdCategoria = datos.IdCategoria;

                // Actualiza el impuesto relacionado con el producto.
                productoActual.Data.IdImpuesto = datos.IdImpuesto;

                // Actualiza el nombre.
                productoActual.Data.Nombre = datos.Nombre;

                // Actualiza la descripción.
                productoActual.Data.Descripcion = datos.Descripcion;

                // Actualiza el código.
                productoActual.Data.Codigo = datos.Codigo;

                // Actualiza el precio de venta.
                productoActual.Data.Precio = datos.Precio;

                // Actualiza el costo del producto.
                productoActual.Data.Costo = datos.Costo;

                // Actualiza la imagen si se recibió una nueva.
                productoActual.Data.Imagen = datos.Imagen;

                // Actualiza el nivel mínimo de inventario.
                productoActual.Data.StockMinimo = datos.StockMinimo;

                // Actualiza si el producto está activo o inactivo.
                productoActual.Data.Estado = datos.Estado;


                // Envía la entidad actualizada al repositorio.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProducto.ModificarAsync(
                        productoActual.Data);


                // Comprueba si ocurrió algún error en la modificación.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el producto modificado dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error indicando cuál producto se intentaba modificar.
                _logger.LogError(
                    ex,
                    "Error al modificar el producto con IdProducto {IdProducto}",
                    datos.IdProducto);

                resultado.Error = ex.Message;
            }

            return resultado;
        }


        // Este método elimina un producto de la base de datos.
        public async Task<Respuesta<bool>> EliminarAsync(Producto datos)
        {
            // Crea una respuesta booleana.
            // True indicará que se eliminó y false que no se pudo eliminar.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca primero el producto utilizando su identificador.
                var producto =
                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                        x => x.IdProducto == datos.IdProducto);

                // Comprueba que el producto realmente exista antes de eliminarlo.
                if (producto.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe el producto que desea eliminar.";

                    return resultado;
                }


                // Envía la entidad encontrada al repositorio para eliminarla.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProducto.EliminarAsync(
                        producto.Data);


                // Si ocurrió un error en acceso a datos, lo devuelve.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Data = false;
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el resultado de la eliminación.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el producto que estaba intentando eliminarse.
                _logger.LogError(
                    ex,
                    "Error al eliminar el producto con IdProducto {IdProducto}",
                    datos.IdProducto);

                // Indica que la eliminación no fue completada.
                resultado.Data = false;

                resultado.Error = ex.Message;
            }

            return resultado;
        }


        // Este método busca productos utilizando el nombre recibido.
        public async Task<Respuesta<IEnumerable<Producto>>> BuscarAsync(
            Producto datos)
        {
            // Crea una respuesta capaz de devolver varios productos.
            var resultado =
                new Respuesta<IEnumerable<Producto>>();

            try
            {
                // Si el nombre llega vacío o nulo, utiliza una cadena vacía.
                // Esto evita errores al utilizar Contains.
                var nombre = datos.Nombre ?? string.Empty;


                // Busca todos los productos cuyo nombre contenga
                // el texto recibido por el usuario.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProducto.BuscarAsync(
                        x => x.Nombre.Contains(nombre));


                // Comprueba si hubo un error al ejecutar la consulta.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los productos encontrados dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra cualquier error producido durante la búsqueda.
                _logger.LogError(
                    ex,
                    "Error al buscar productos por nombre.");

                resultado.Error = ex.Message;
            }

            return resultado;
        }


        // Este método obtiene un producto específico usando IdProducto.
        public async Task<Respuesta<Producto>> ObtenerAsync(Producto datos)
        {
            // Crea una respuesta para devolver un único producto.
            var resultado = new Respuesta<Producto>();

            try
            {
                // Busca en la base de datos el producto cuyo IdProducto
                // sea igual al recibido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                        x => x.IdProducto == datos.IdProducto);


                // Comprueba si el producto fue encontrado.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Producto no encontrado.";

                    return resultado;
                }


                // Guarda el producto encontrado dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el identificador que produjo el error.
                _logger.LogError(
                    ex,
                    "Error al obtener el producto con IdProducto {IdProducto}",
                    datos.IdProducto);

                resultado.Error = ex.Message;
            }

            return resultado;
        }
    }
}