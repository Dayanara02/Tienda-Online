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
    // Contiene la lógica de negocio de las evaluaciones.
    public class EvaluacionProductoLN : IEvaluacionProductoLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<EvaluacionProductoLN> _logger;


        // Recibe las dependencias necesarias.
        public EvaluacionProductoLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<EvaluacionProductoLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra una nueva evaluación.
        public async Task<Respuesta<EvaluacionProducto>> InsertarAsync(
            EvaluacionProducto datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<EvaluacionProducto>();

            try
            {
                // Valida el producto.
                if (datos.IdProducto <= 0)
                {
                    resultado.Error =
                        "Debe indicar un producto válido.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido.";

                    return resultado;
                }


                // Valida la calificación.
                if (datos.Calificacion < 1 ||
                    datos.Calificacion > 5)
                {
                    resultado.Error =
                        "La calificación debe estar entre 1 y 5.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaEvaluacion == default)
                {
                    datos.FechaEvaluacion = DateTime.Now;
                }


                // Guarda la evaluación.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TEvaluacionProducto.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la evaluación registrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar una evaluación.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todas las evaluaciones.
        public async Task<Respuesta<IEnumerable<EvaluacionProducto>>> ListarAsync()
        {
            // Crea la respuesta con varias evaluaciones.
            var resultado =
                new Respuesta<IEnumerable<EvaluacionProducto>>();

            try
            {
                // Consulta todas las evaluaciones.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TEvaluacionProducto.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las evaluaciones encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar las evaluaciones.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica una evaluación existente.
        public async Task<Respuesta<EvaluacionProducto>> ModificarAsync(
            EvaluacionProducto datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<EvaluacionProducto>();

            try
            {
                // Busca la evaluación.
                var evaluacionActual =
                    await _unidadDeTrabajo.TEvaluacionProducto.ObtenerEntidadAsync(
                        x => x.IdEvaluacion ==
                             datos.IdEvaluacion);


                // Comprueba que exista.
                if (evaluacionActual.Data == null)
                {
                    resultado.Error =
                        "No existe la evaluación que desea modificar.";

                    return resultado;
                }


                // Valida el producto.
                if (datos.IdProducto <= 0)
                {
                    resultado.Error =
                        "El producto no es válido.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "El usuario no es válido.";

                    return resultado;
                }


                // Valida la calificación.
                if (datos.Calificacion < 1 ||
                    datos.Calificacion > 5)
                {
                    resultado.Error =
                        "La calificación debe estar entre 1 y 5.";

                    return resultado;
                }


                // Actualiza el producto.
                evaluacionActual.Data.IdProducto =
                    datos.IdProducto;

                // Actualiza el usuario.
                evaluacionActual.Data.IdUsuario =
                    datos.IdUsuario;

                // Actualiza la calificación.
                evaluacionActual.Data.Calificacion =
                    datos.Calificacion;

                // Actualiza el comentario.
                evaluacionActual.Data.Comentario =
                    datos.Comentario;

                // Actualiza la fecha.
                evaluacionActual.Data.FechaEvaluacion =
                    datos.FechaEvaluacion;

                // Actualiza el estado.
                evaluacionActual.Data.Estado =
                    datos.Estado;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TEvaluacionProducto.ModificarAsync(
                        evaluacionActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la evaluación modificada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar la evaluación {IdEvaluacion}",
                    datos.IdEvaluacion);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina una evaluación.
        public async Task<Respuesta<bool>> EliminarAsync(
            EvaluacionProducto datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca la evaluación.
                var evaluacion =
                    await _unidadDeTrabajo.TEvaluacionProducto.ObtenerEntidadAsync(
                        x => x.IdEvaluacion ==
                             datos.IdEvaluacion);


                // Comprueba que exista.
                if (evaluacion.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe la evaluación que desea eliminar.";

                    return resultado;
                }


                // Elimina la evaluación.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TEvaluacionProducto.EliminarAsync(
                        evaluacion.Data);


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
                    "Error al eliminar la evaluación {IdEvaluacion}",
                    datos.IdEvaluacion);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca evaluaciones por comentario.
        public async Task<Respuesta<IEnumerable<EvaluacionProducto>>> BuscarAsync(
            EvaluacionProducto datos)
        {
            // Crea la respuesta con varias evaluaciones.
            var resultado =
                new Respuesta<IEnumerable<EvaluacionProducto>>();

            try
            {
                // Evita errores si el comentario viene nulo.
                var comentario =
                    datos.Comentario ?? string.Empty;


                // Busca evaluaciones por comentario.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TEvaluacionProducto.BuscarAsync(
                        x => x.Comentario != null &&
                             x.Comentario.Contains(comentario));


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las evaluaciones encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar evaluaciones.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene una evaluación por su identificador.
        public async Task<Respuesta<EvaluacionProducto>> ObtenerAsync(
            EvaluacionProducto datos)
        {
            // Crea la respuesta de la operación.
            var resultado =
                new Respuesta<EvaluacionProducto>();

            try
            {
                // Busca la evaluación.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TEvaluacionProducto.ObtenerEntidadAsync(
                        x => x.IdEvaluacion ==
                             datos.IdEvaluacion);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Evaluación no encontrada.";

                    return resultado;
                }


                // Guarda la evaluación encontrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener la evaluación {IdEvaluacion}",
                    datos.IdEvaluacion);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}