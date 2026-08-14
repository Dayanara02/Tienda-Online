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
    // Contiene la lógica de negocio de las notificaciones.
    public class NotificacionLN : INotificacionLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<NotificacionLN> _logger;


        // Recibe las dependencias necesarias.
        public NotificacionLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<NotificacionLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra una nueva notificación.
        public async Task<Respuesta<Notificacion>> InsertarAsync(
            Notificacion datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Notificacion>();

            try
            {
                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido.";

                    return resultado;
                }


                // Valida el título.
                if (string.IsNullOrWhiteSpace(datos.Titulo))
                {
                    resultado.Error =
                        "Debe indicar el título de la notificación.";

                    return resultado;
                }


                // Valida el mensaje.
                if (string.IsNullOrWhiteSpace(datos.Mensaje))
                {
                    resultado.Error =
                        "Debe indicar el mensaje de la notificación.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaCreacion == default)
                {
                    datos.FechaCreacion = DateTime.Now;
                }


                // Guarda la notificación.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TNotificacion.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la notificación registrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar una notificación.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todas las notificaciones.
        public async Task<Respuesta<IEnumerable<Notificacion>>> ListarAsync()
        {
            // Crea la respuesta con varias notificaciones.
            var resultado =
                new Respuesta<IEnumerable<Notificacion>>();

            try
            {
                // Consulta todas las notificaciones.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TNotificacion.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las notificaciones encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar las notificaciones.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica una notificación existente.
        public async Task<Respuesta<Notificacion>> ModificarAsync(
            Notificacion datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Notificacion>();

            try
            {
                // Busca la notificación.
                var notificacionActual =
                    await _unidadDeTrabajo.TNotificacion.ObtenerEntidadAsync(
                        x => x.IdNotificacion ==
                             datos.IdNotificacion);


                // Comprueba que exista.
                if (notificacionActual.Data == null)
                {
                    resultado.Error =
                        "No existe la notificación que desea modificar.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "El usuario no es válido.";

                    return resultado;
                }


                // Valida el título.
                if (string.IsNullOrWhiteSpace(datos.Titulo))
                {
                    resultado.Error =
                        "Debe indicar el título de la notificación.";

                    return resultado;
                }


                // Valida el mensaje.
                if (string.IsNullOrWhiteSpace(datos.Mensaje))
                {
                    resultado.Error =
                        "Debe indicar el mensaje de la notificación.";

                    return resultado;
                }


                // Actualiza el usuario.
                notificacionActual.Data.IdUsuario =
                    datos.IdUsuario;

                // Actualiza el título.
                notificacionActual.Data.Titulo =
                    datos.Titulo;

                // Actualiza el mensaje.
                notificacionActual.Data.Mensaje =
                    datos.Mensaje;

                // Actualiza el tipo.
                notificacionActual.Data.Tipo =
                    datos.Tipo;

                // Actualiza la fecha.
                notificacionActual.Data.FechaCreacion =
                    datos.FechaCreacion;

                // Actualiza si fue leída.
                notificacionActual.Data.Leida =
                    datos.Leida;

                // Actualiza el estado.
                notificacionActual.Data.Estado =
                    datos.Estado;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TNotificacion.ModificarAsync(
                        notificacionActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la notificación modificada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar la notificación {IdNotificacion}",
                    datos.IdNotificacion);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina una notificación.
        public async Task<Respuesta<bool>> EliminarAsync(
            Notificacion datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca la notificación.
                var notificacion =
                    await _unidadDeTrabajo.TNotificacion.ObtenerEntidadAsync(
                        x => x.IdNotificacion ==
                             datos.IdNotificacion);


                // Comprueba que exista.
                if (notificacion.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe la notificación que desea eliminar.";

                    return resultado;
                }


                // Elimina la notificación.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TNotificacion.EliminarAsync(
                        notificacion.Data);


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
                    "Error al eliminar la notificación {IdNotificacion}",
                    datos.IdNotificacion);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca notificaciones por título.
        public async Task<Respuesta<IEnumerable<Notificacion>>> BuscarAsync(
            Notificacion datos)
        {
            // Crea la respuesta con varias notificaciones.
            var resultado =
                new Respuesta<IEnumerable<Notificacion>>();

            try
            {
                // Evita errores si el título viene nulo.
                var titulo =
                    datos.Titulo ?? string.Empty;


                // Busca notificaciones por título.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TNotificacion.BuscarAsync(
                        x => x.Titulo.Contains(titulo));


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las notificaciones encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar notificaciones.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene una notificación por su identificador.
        public async Task<Respuesta<Notificacion>> ObtenerAsync(
            Notificacion datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Notificacion>();

            try
            {
                // Busca la notificación.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TNotificacion.ObtenerEntidadAsync(
                        x => x.IdNotificacion ==
                             datos.IdNotificacion);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Notificación no encontrada.";

                    return resultado;
                }


                // Guarda la notificación encontrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener la notificación {IdNotificacion}",
                    datos.IdNotificacion);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}