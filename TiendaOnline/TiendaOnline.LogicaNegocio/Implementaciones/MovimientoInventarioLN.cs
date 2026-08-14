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
    // Contiene la lógica de negocio de los movimientos de inventario.
    public class MovimientoInventarioLN : IMovimientoInventarioLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<MovimientoInventarioLN> _logger;


        // Recibe las dependencias necesarias.
        public MovimientoInventarioLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<MovimientoInventarioLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra un nuevo movimiento de inventario.
        public async Task<Respuesta<MovimientoInventario>> InsertarAsync(
            MovimientoInventario datos)
        {
            // Crea la respuesta de la operación.
            var resultado =
                new Respuesta<MovimientoInventario>();

            try
            {
                // Valida el inventario.
                if (datos.IdInventario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un inventario válido.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido.";

                    return resultado;
                }


                // Valida el tipo de movimiento.
                if (string.IsNullOrWhiteSpace(
                    datos.TipoMovimiento))
                {
                    resultado.Error =
                        "Debe indicar el tipo de movimiento.";

                    return resultado;
                }


                // Valida la cantidad.
                if (datos.Cantidad <= 0)
                {
                    resultado.Error =
                        "La cantidad debe ser mayor que cero.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaMovimiento == default)
                {
                    datos.FechaMovimiento = DateTime.Now;
                }


                // Guarda el movimiento.
                var respuestaRepositorio =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .InsertarAsync(datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el movimiento registrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar un movimiento de inventario.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todos los movimientos de inventario.
        public async Task<Respuesta<IEnumerable<MovimientoInventario>>> ListarAsync()
        {
            // Crea la respuesta con varios movimientos.
            var resultado =
                new Respuesta<IEnumerable<MovimientoInventario>>();

            try
            {
                // Consulta todos los movimientos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los movimientos encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar los movimientos de inventario.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica un movimiento existente.
        public async Task<Respuesta<MovimientoInventario>> ModificarAsync(
            MovimientoInventario datos)
        {
            // Crea la respuesta de la operación.
            var resultado =
                new Respuesta<MovimientoInventario>();

            try
            {
                // Busca el movimiento.
                var movimientoActual =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .ObtenerEntidadAsync(
                            x => x.IdMovimiento ==
                                 datos.IdMovimiento);


                // Comprueba que exista.
                if (movimientoActual.Data == null)
                {
                    resultado.Error =
                        "No existe el movimiento que desea modificar.";

                    return resultado;
                }


                // Valida el inventario.
                if (datos.IdInventario <= 0)
                {
                    resultado.Error =
                        "El inventario no es válido.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "El usuario no es válido.";

                    return resultado;
                }


                // Valida el tipo de movimiento.
                if (string.IsNullOrWhiteSpace(
                    datos.TipoMovimiento))
                {
                    resultado.Error =
                        "Debe indicar el tipo de movimiento.";

                    return resultado;
                }


                // Valida la cantidad.
                if (datos.Cantidad <= 0)
                {
                    resultado.Error =
                        "La cantidad debe ser mayor que cero.";

                    return resultado;
                }


                // Actualiza el inventario.
                movimientoActual.Data.IdInventario =
                    datos.IdInventario;

                // Actualiza el usuario.
                movimientoActual.Data.IdUsuario =
                    datos.IdUsuario;

                // Actualiza el tipo de movimiento.
                movimientoActual.Data.TipoMovimiento =
                    datos.TipoMovimiento;

                // Actualiza la cantidad.
                movimientoActual.Data.Cantidad =
                    datos.Cantidad;

                // Actualiza el motivo.
                movimientoActual.Data.Motivo =
                    datos.Motivo;

                // Actualiza la fecha.
                movimientoActual.Data.FechaMovimiento =
                    datos.FechaMovimiento;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .ModificarAsync(
                            movimientoActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el movimiento modificado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar el movimiento {IdMovimiento}",
                    datos.IdMovimiento);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina un movimiento de inventario.
        public async Task<Respuesta<bool>> EliminarAsync(
            MovimientoInventario datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca el movimiento.
                var movimiento =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .ObtenerEntidadAsync(
                            x => x.IdMovimiento ==
                                 datos.IdMovimiento);


                // Comprueba que exista.
                if (movimiento.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe el movimiento que desea eliminar.";

                    return resultado;
                }


                // Elimina el movimiento.
                var respuestaRepositorio =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .EliminarAsync(
                            movimiento.Data);


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
                    "Error al eliminar el movimiento {IdMovimiento}",
                    datos.IdMovimiento);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca movimientos por su tipo.
        public async Task<Respuesta<IEnumerable<MovimientoInventario>>> BuscarAsync(
            MovimientoInventario datos)
        {
            // Crea la respuesta con varios movimientos.
            var resultado =
                new Respuesta<IEnumerable<MovimientoInventario>>();

            try
            {
                // Evita errores si el tipo viene nulo.
                var tipoMovimiento =
                    datos.TipoMovimiento ?? string.Empty;


                // Busca por tipo de movimiento.
                var respuestaRepositorio =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .BuscarAsync(
                            x => x.TipoMovimiento.Contains(
                                tipoMovimiento));


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los movimientos encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar movimientos de inventario.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene un movimiento por su identificador.
        public async Task<Respuesta<MovimientoInventario>> ObtenerAsync(
            MovimientoInventario datos)
        {
            // Crea la respuesta de la operación.
            var resultado =
                new Respuesta<MovimientoInventario>();

            try
            {
                // Busca el movimiento.
                var respuestaRepositorio =
                    await _unidadDeTrabajo
                        .TMovimientoInventario
                        .ObtenerEntidadAsync(
                            x => x.IdMovimiento ==
                                 datos.IdMovimiento);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Movimiento de inventario no encontrado.";

                    return resultado;
                }


                // Guarda el movimiento encontrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener el movimiento {IdMovimiento}",
                    datos.IdMovimiento);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}