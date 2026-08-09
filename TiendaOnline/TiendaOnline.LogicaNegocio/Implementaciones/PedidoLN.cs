using Microsoft.Extensions.Logging;
using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Dominio.InterfacesAD;
using TiendaOnline.Dominio.InterfacesLN;
using TiendaOnline.Utilidades;

namespace TiendaOnline.LogicaNegocio.Implementaciones
{
    // Esta clase contiene las reglas de negocio generales de los pedidos.
    // Implementa IPedidoLN para cumplir con las operaciones definidas
    // para insertar, consultar, modificar, eliminar y buscar pedidos.
    public class PedidoLN : IPedidoLN
    {
        // Guarda la unidad de trabajo.
        // Por medio de ella se obtiene el repositorio TPedido,
        // evitando acceder directamente al contexto de Entity Framework.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores que ocurran dentro de la lógica de pedidos.
        // Esto ayuda a identificar problemas durante la ejecución del sistema.
        private readonly ILogger<PedidoLN> _logger;


        // El constructor recibe las dependencias que necesita esta clase.
        // Estas dependencias pueden ser entregadas por inyección de dependencias.
        public PedidoLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<PedidoLN> logger)
        {
            // Guarda la unidad de trabajo recibida para utilizar TPedido.
            _unidadDeTrabajo = unidadDeTrabajo;

            // Guarda el logger para poder registrar errores.
            _logger = logger;
        }


        // Este método registra un nuevo pedido en la base de datos.
        public async Task<Respuesta<Pedido>> InsertarAsync(Pedido datos)
        {
            // Crea una respuesta donde se devolverá
            // el pedido registrado o un mensaje de error.
            var resultado = new Respuesta<Pedido>();

            try
            {
                // Verifica que el pedido pertenezca a un usuario válido.
                // Un IdUsuario menor o igual a cero no representa un usuario válido.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido para realizar el pedido.";

                    return resultado;
                }


                // Verifica que el subtotal no sea negativo.
                // Un pedido no debería tener un subtotal menor que cero.
                if (datos.Subtotal < 0)
                {
                    resultado.Error =
                        "El subtotal del pedido no puede ser negativo.";

                    return resultado;
                }


                // Verifica que el impuesto no tenga un valor negativo.
                if (datos.Impuesto < 0)
                {
                    resultado.Error =
                        "El impuesto del pedido no puede ser negativo.";

                    return resultado;
                }


                // Verifica que el descuento no sea negativo.
                if (datos.Descuento < 0)
                {
                    resultado.Error =
                        "El descuento del pedido no puede ser negativo.";

                    return resultado;
                }


                // Verifica que el total final del pedido sea válido.
                if (datos.Total < 0)
                {
                    resultado.Error =
                        "El total del pedido no puede ser negativo.";

                    return resultado;
                }


                // Si no se recibió una fecha válida,
                // se utiliza la fecha y hora actual.
                if (datos.FechaPedido == default)
                {
                    datos.FechaPedido = DateTime.Now;
                }


                // Si el estado viene vacío, se asigna un estado inicial.
                // Esto evita guardar pedidos sin información sobre su estado.
                if (string.IsNullOrWhiteSpace(datos.Estado))
                {
                    datos.Estado = "Pendiente";
                }


                // Envía el pedido al repositorio para insertarlo
                // realmente en la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.InsertarAsync(datos);


                // Verifica si el repositorio produjo algún error.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    // Copia el error para devolverlo a la capa superior.
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda en la respuesta el pedido que fue registrado.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error indicando el usuario
                // para el cual se intentaba crear el pedido.
                _logger.LogError(
                    ex,
                    "Error al insertar un pedido para el usuario {IdUsuario}",
                    datos.IdUsuario);

                // Guarda el mensaje del error dentro de la respuesta.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado final de la operación.
            return resultado;
        }


        // Este método obtiene todos los pedidos registrados.
        public async Task<Respuesta<IEnumerable<Pedido>>> ListarAsync()
        {
            // Crea una respuesta que puede almacenar varios pedidos.
            var resultado =
                new Respuesta<IEnumerable<Pedido>>();

            try
            {
                // Solicita al repositorio todos los registros
                // existentes en la tabla de pedidos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ListarAsync();


                // Comprueba si ocurrió algún error durante la consulta.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la lista de pedidos obtenida.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra cualquier error ocurrido al listar los pedidos.
                _logger.LogError(
                    ex,
                    "Error al listar los pedidos.");

                // Guarda el mensaje del error para devolverlo.
                resultado.Error = ex.Message;
            }

            // Devuelve los pedidos encontrados o el error.
            return resultado;
        }


        // Este método modifica los datos generales de un pedido existente.
        public async Task<Respuesta<Pedido>> ModificarAsync(Pedido datos)
        {
            // Crea la respuesta que devolverá el pedido modificado.
            var resultado = new Respuesta<Pedido>();

            try
            {
                // Busca primero el pedido utilizando su IdPedido.
                var pedidoActual =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.IdPedido == datos.IdPedido);


                // Comprueba que el pedido exista antes de intentar modificarlo.
                if (pedidoActual.Data == null)
                {
                    resultado.Error =
                        "No existe el pedido que desea modificar.";

                    return resultado;
                }


                // Valida que el usuario relacionado con el pedido sea válido.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "El usuario relacionado con el pedido no es válido.";

                    return resultado;
                }


                // Valida que los valores monetarios no sean negativos.
                if (datos.Subtotal < 0 ||
                    datos.Impuesto < 0 ||
                    datos.Descuento < 0 ||
                    datos.Total < 0)
                {
                    resultado.Error =
                        "Los valores monetarios del pedido no pueden ser negativos.";

                    return resultado;
                }


                // Actualiza el usuario relacionado con el pedido.
                pedidoActual.Data.IdUsuario = datos.IdUsuario;

                // Actualiza la fecha del pedido.
                pedidoActual.Data.FechaPedido = datos.FechaPedido;

                // Actualiza el estado textual del pedido.
                pedidoActual.Data.Estado = datos.Estado;

                // Actualiza el subtotal antes de impuestos y descuentos.
                pedidoActual.Data.Subtotal = datos.Subtotal;

                // Actualiza el monto correspondiente a impuestos.
                pedidoActual.Data.Impuesto = datos.Impuesto;

                // Actualiza el descuento aplicado al pedido.
                pedidoActual.Data.Descuento = datos.Descuento;

                // Actualiza el total final que debe pagar el usuario.
                pedidoActual.Data.Total = datos.Total;

                // Actualiza la dirección en la que se entregará el pedido.
                pedidoActual.Data.DireccionEntrega = datos.DireccionEntrega;

                // Actualiza el identificador del estado del pedido.
                pedidoActual.Data.IdEstadoPedido = datos.IdEstadoPedido;


                // Envía la entidad actualizada al repositorio
                // para guardar los cambios en la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ModificarAsync(
                        pedidoActual.Data);


                // Verifica si el repositorio produjo algún error.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda en la respuesta el pedido ya modificado.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra cuál pedido se intentaba modificar
                // cuando ocurrió el error.
                _logger.LogError(
                    ex,
                    "Error al modificar el pedido con IdPedido {IdPedido}",
                    datos.IdPedido);

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado de la modificación.
            return resultado;
        }


        // Este método elimina un pedido existente.
        public async Task<Respuesta<bool>> EliminarAsync(Pedido datos)
        {
            // Crea una respuesta booleana.
            // True significa que se eliminó y false que la operación falló.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca primero el pedido utilizando su identificador.
                var pedido =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.IdPedido == datos.IdPedido);


                // Si no encuentra el pedido, no se puede eliminar.
                if (pedido.Data == null)
                {
                    // Indica que la eliminación no fue realizada.
                    resultado.Data = false;

                    // Explica por qué no se pudo realizar.
                    resultado.Error =
                        "No existe el pedido que desea eliminar.";

                    return resultado;
                }


                // Envía el pedido encontrado al repositorio
                // para eliminarlo de la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.EliminarAsync(
                        pedido.Data);


                // Comprueba si ocurrió un error durante la eliminación.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Data = false;
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el resultado devuelto por el repositorio.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el IdPedido que produjo el problema.
                _logger.LogError(
                    ex,
                    "Error al eliminar el pedido con IdPedido {IdPedido}",
                    datos.IdPedido);

                // Indica que la operación no se completó.
                resultado.Data = false;

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado de la eliminación.
            return resultado;
        }


        // Este método permite buscar pedidos.
        // En este caso utiliza el estado como criterio de búsqueda.
        public async Task<Respuesta<IEnumerable<Pedido>>> BuscarAsync(
            Pedido datos)
        {
            // Crea una respuesta capaz de contener varios pedidos.
            var resultado =
                new Respuesta<IEnumerable<Pedido>>();

            try
            {
                // Si el estado viene nulo, utiliza una cadena vacía.
                // Esto evita errores al utilizar Contains.
                var estado = datos.Estado ?? string.Empty;


                // Busca todos los pedidos cuyo estado contenga
                // el texto recibido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.BuscarAsync(
                        x => x.Estado.Contains(estado));


                // Comprueba si hubo un error durante la búsqueda.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda en la respuesta todos los pedidos encontrados.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido durante la búsqueda.
                _logger.LogError(
                    ex,
                    "Error al buscar pedidos.");

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve los pedidos encontrados o el error.
            return resultado;
        }


        // Este método obtiene un pedido específico utilizando su IdPedido.
        public async Task<Respuesta<Pedido>> ObtenerAsync(Pedido datos)
        {
            // Crea una respuesta para devolver un único pedido.
            var resultado = new Respuesta<Pedido>();

            try
            {
                // Busca en la base de datos el pedido cuyo identificador
                // coincida con el IdPedido recibido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.IdPedido == datos.IdPedido);


                // Comprueba si realmente se encontró el pedido.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Pedido no encontrado.";

                    return resultado;
                }


                // Guarda el pedido encontrado dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra cuál pedido produjo el error.
                _logger.LogError(
                    ex,
                    "Error al obtener el pedido con IdPedido {IdPedido}",
                    datos.IdPedido);

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve el pedido encontrado o el error.
            return resultado;
        }
    }
}