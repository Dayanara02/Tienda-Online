// Permite registrar errores.
using Microsoft.Extensions.Logging;

// Importa las entidades.
using TiendaOnline.Dominio.Entidades;

// Importa la unidad de trabajo.
using TiendaOnline.Dominio.InterfacesAD;

// Importa la interfaz de lógica de negocio.
using TiendaOnline.Dominio.InterfacesLN;

// Importa la clase Respuesta.
using TiendaOnline.Utilidades;

namespace TiendaOnline.LogicaNegocio.Implementaciones
{
    // Contiene la lógica de negocio de los pedidos.
    public class PedidoLN : IPedidoLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<PedidoLN> _logger;


        // Recibe las dependencias necesarias.
        public PedidoLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<PedidoLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra un nuevo pedido.
        public async Task<Respuesta<Pedido>> InsertarAsync(
            Pedido datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Pedido>();

            try
            {
                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido para realizar el pedido.";

                    return resultado;
                }


                // Valida el subtotal.
                if (datos.Subtotal < 0)
                {
                    resultado.Error =
                        "El subtotal del pedido no puede ser negativo.";

                    return resultado;
                }


                // Valida el impuesto.
                if (datos.Impuesto < 0)
                {
                    resultado.Error =
                        "El impuesto del pedido no puede ser negativo.";

                    return resultado;
                }


                // Valida el descuento.
                if (datos.Descuento < 0)
                {
                    resultado.Error =
                        "El descuento del pedido no puede ser negativo.";

                    return resultado;
                }


                // Valida el total.
                if (datos.Total < 0)
                {
                    resultado.Error =
                        "El total del pedido no puede ser negativo.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaPedido == default)
                {
                    datos.FechaPedido = DateTime.Now;
                }


                // Asigna un estado inicial.
                if (string.IsNullOrWhiteSpace(datos.Estado))
                {
                    datos.Estado = "Pendiente";
                }


                // Guarda el pedido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el pedido registrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar un pedido para el usuario {IdUsuario}",
                    datos.IdUsuario);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todos los pedidos.
        public async Task<Respuesta<IEnumerable<Pedido>>> ListarAsync()
        {
            // Crea la respuesta con varios pedidos.
            var resultado =
                new Respuesta<IEnumerable<Pedido>>();

            try
            {
                // Consulta todos los pedidos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los pedidos encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar los pedidos.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica un pedido existente.
        public async Task<Respuesta<Pedido>> ModificarAsync(
            Pedido datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Pedido>();

            try
            {
                // Busca el pedido.
                var pedidoActual =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.IdPedido ==
                             datos.IdPedido);


                // Comprueba que exista.
                if (pedidoActual.Data == null)
                {
                    resultado.Error =
                        "No existe el pedido que desea modificar.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "El usuario relacionado con el pedido no es válido.";

                    return resultado;
                }


                // Valida los valores monetarios.
                if (datos.Subtotal < 0 ||
                    datos.Impuesto < 0 ||
                    datos.Descuento < 0 ||
                    datos.Total < 0)
                {
                    resultado.Error =
                        "Los valores monetarios del pedido no pueden ser negativos.";

                    return resultado;
                }


                // Actualiza el usuario.
                pedidoActual.Data.IdUsuario =
                    datos.IdUsuario;

                // Actualiza la fecha.
                pedidoActual.Data.FechaPedido =
                    datos.FechaPedido;

                // Actualiza el estado.
                pedidoActual.Data.Estado =
                    datos.Estado;

                // Actualiza el subtotal.
                pedidoActual.Data.Subtotal =
                    datos.Subtotal;

                // Actualiza el impuesto.
                pedidoActual.Data.Impuesto =
                    datos.Impuesto;

                // Actualiza el descuento.
                pedidoActual.Data.Descuento =
                    datos.Descuento;

                // Actualiza el total.
                pedidoActual.Data.Total =
                    datos.Total;

                // Actualiza la dirección de entrega.
                pedidoActual.Data.DireccionEntrega =
                    datos.DireccionEntrega;

                // Actualiza el estado relacionado.
                pedidoActual.Data.IdEstadoPedido =
                    datos.IdEstadoPedido;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ModificarAsync(
                        pedidoActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el pedido modificado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar el pedido {IdPedido}",
                    datos.IdPedido);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina un pedido.
        public async Task<Respuesta<bool>> EliminarAsync(
            Pedido datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca el pedido.
                var pedido =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.IdPedido ==
                             datos.IdPedido);


                // Comprueba que exista.
                if (pedido.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe el pedido que desea eliminar.";

                    return resultado;
                }


                // Elimina el pedido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.EliminarAsync(
                        pedido.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Data = false;

                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el resultado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al eliminar el pedido {IdPedido}",
                    datos.IdPedido);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca pedidos por estado.
        public async Task<Respuesta<IEnumerable<Pedido>>> BuscarAsync(
            Pedido datos)
        {
            // Crea la respuesta con varios pedidos.
            var resultado =
                new Respuesta<IEnumerable<Pedido>>();

            try
            {
                // Evita errores si el estado viene nulo.
                var estado =
                    datos.Estado ?? string.Empty;


                // Busca pedidos por estado.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.BuscarAsync(
                        x => x.Estado.Contains(estado));


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los pedidos encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar pedidos.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene un pedido por su identificador.
        public async Task<Respuesta<Pedido>> ObtenerAsync(
            Pedido datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Pedido>();

            try
            {
                // Busca el pedido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.IdPedido ==
                             datos.IdPedido);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Pedido no encontrado.";

                    return resultado;
                }


                // Guarda el pedido encontrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener el pedido {IdPedido}",
                    datos.IdPedido);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}