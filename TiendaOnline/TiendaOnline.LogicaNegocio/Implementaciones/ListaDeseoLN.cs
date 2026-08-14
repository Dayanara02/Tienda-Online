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
    // Contiene la lógica de negocio de las listas de deseos.
    public class ListaDeseoLN : IListaDeseoLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<ListaDeseoLN> _logger;


        // Recibe las dependencias necesarias.
        public ListaDeseoLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<ListaDeseoLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra una nueva lista de deseos.
        public async Task<Respuesta<ListaDeseo>> InsertarAsync(
            ListaDeseo datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<ListaDeseo>();

            try
            {
                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaCreacion == default)
                {
                    datos.FechaCreacion = DateTime.Now;
                }


                // Verifica si el usuario ya tiene una lista.
                var listaExistente =
                    await _unidadDeTrabajo.TListaDeseo.ObtenerEntidadAsync(
                        x => x.IdUsuario == datos.IdUsuario);


                // Evita crear más de una lista por usuario.
                if (listaExistente.Data != null)
                {
                    resultado.Error =
                        "El usuario ya tiene una lista de deseos.";

                    return resultado;
                }


                // Guarda la lista de deseos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TListaDeseo.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la lista registrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar la lista de deseos del usuario {IdUsuario}",
                    datos.IdUsuario);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todas las listas de deseos.
        public async Task<Respuesta<IEnumerable<ListaDeseo>>> ListarAsync()
        {
            // Crea la respuesta con varias listas.
            var resultado =
                new Respuesta<IEnumerable<ListaDeseo>>();

            try
            {
                // Consulta todas las listas.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TListaDeseo.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las listas encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar las listas de deseos.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica una lista de deseos.
        public async Task<Respuesta<ListaDeseo>> ModificarAsync(
            ListaDeseo datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<ListaDeseo>();

            try
            {
                // Busca la lista.
                var listaActual =
                    await _unidadDeTrabajo.TListaDeseo.ObtenerEntidadAsync(
                        x => x.IdListaDeseos ==
                             datos.IdListaDeseos);


                // Comprueba que exista.
                if (listaActual.Data == null)
                {
                    resultado.Error =
                        "No existe la lista de deseos que desea modificar.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "El usuario no es válido.";

                    return resultado;
                }


                // Actualiza el usuario.
                listaActual.Data.IdUsuario =
                    datos.IdUsuario;

                // Actualiza la fecha.
                listaActual.Data.FechaCreacion =
                    datos.FechaCreacion;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TListaDeseo.ModificarAsync(
                        listaActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la lista modificada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar la lista {IdListaDeseos}",
                    datos.IdListaDeseos);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina una lista de deseos.
        public async Task<Respuesta<bool>> EliminarAsync(
            ListaDeseo datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca la lista.
                var lista =
                    await _unidadDeTrabajo.TListaDeseo.ObtenerEntidadAsync(
                        x => x.IdListaDeseos ==
                             datos.IdListaDeseos);


                // Comprueba que exista.
                if (lista.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe la lista de deseos que desea eliminar.";

                    return resultado;
                }


                // Elimina la lista.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TListaDeseo.EliminarAsync(
                        lista.Data);


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
                    "Error al eliminar la lista {IdListaDeseos}",
                    datos.IdListaDeseos);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca listas por usuario.
        public async Task<Respuesta<IEnumerable<ListaDeseo>>> BuscarAsync(
            ListaDeseo datos)
        {
            // Crea la respuesta con varias listas.
            var resultado =
                new Respuesta<IEnumerable<ListaDeseo>>();

            try
            {
                // Busca por identificador del usuario.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TListaDeseo.BuscarAsync(
                        x => datos.IdUsuario <= 0 ||
                             x.IdUsuario == datos.IdUsuario);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las listas encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar listas de deseos.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene una lista por su identificador.
        public async Task<Respuesta<ListaDeseo>> ObtenerAsync(
            ListaDeseo datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<ListaDeseo>();

            try
            {
                // Busca la lista.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TListaDeseo.ObtenerEntidadAsync(
                        x => x.IdListaDeseos ==
                             datos.IdListaDeseos);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Lista de deseos no encontrada.";

                    return resultado;
                }


                // Guarda la lista encontrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener la lista {IdListaDeseos}",
                    datos.IdListaDeseos);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}