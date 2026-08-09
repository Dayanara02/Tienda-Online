using Microsoft.Extensions.Logging;
using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Dominio.InterfacesAD;
using TiendaOnline.Dominio.InterfacesLN;
using TiendaOnline.Utilidades;

namespace TiendaOnline.LogicaNegocio.Implementaciones
{
    // Esta clase contiene la lógica de negocio relacionada con las categorías.
    // Implementa ICategoriaLN para cumplir con las operaciones definidas en la interfaz.
    public class CategoriaLN : ICategoriaLN
    {
        // Guarda la unidad de trabajo.
        // Por medio de ella se accede al repositorio de categorías.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores que ocurran dentro de esta clase.
        private readonly ILogger<CategoriaLN> _logger;


        // El constructor recibe las dependencias necesarias.
        // Estas se asignan mediante inyección de dependencias.
        public CategoriaLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<CategoriaLN> logger)
        {
            // Guarda la unidad de trabajo para poder utilizar TCategoria.
            _unidadDeTrabajo = unidadDeTrabajo;

            // Guarda el logger para registrar errores.
            _logger = logger;
        }


        // Este método registra una nueva categoría.
        public async Task<Respuesta<Categorium>> InsertarAsync(Categorium datos)
        {
            // Crea el objeto que devolverá la categoría o un mensaje de error.
            var resultado = new Respuesta<Categorium>();

            try
            {
                // Busca si ya existe una categoría con el mismo nombre.
                var categoriaExistente =
                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(
                        x => x.Nombre == datos.Nombre);

                // Si encuentra una categoría, evita registrar un duplicado.
                if (categoriaExistente.Data != null)
                {
                    // Guarda la categoría existente dentro de la respuesta.
                    resultado.Data = categoriaExistente.Data;

                    // Indica por qué no se realizó la inserción.
                    resultado.Error =
                        "Ya existe una categoría registrada con ese nombre.";

                    // Finaliza el método sin insertar una nueva categoría.
                    return resultado;
                }

                // Valida que el nombre tenga información.
                if (string.IsNullOrWhiteSpace(datos.Nombre))
                {
                    resultado.Error =
                        "El nombre de la categoría es obligatorio.";

                    return resultado;
                }

                // Verifica que la categoría tenga una familia válida.
                if (datos.IdFamilia <= 0)
                {
                    resultado.Error =
                        "Debe seleccionar una familia válida.";

                    return resultado;
                }

                // Envía la categoría al repositorio para guardarla en la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCategoria.InsertarAsync(datos);

                // Comprueba si el repositorio devolvió un error.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }

                // Guarda en la respuesta la categoría registrada.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error indicando qué categoría se intentaba insertar.
                _logger.LogError(
                    ex,
                    "Error al insertar la categoría {Nombre}",
                    datos.Nombre);

                // Guarda el mensaje del error dentro de la respuesta.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado final.
            return resultado;
        }


        // Este método obtiene todas las categorías registradas.
        public async Task<Respuesta<IEnumerable<Categorium>>> ListarAsync()
        {
            // Crea una respuesta capaz de devolver varias categorías.
            var resultado =
                new Respuesta<IEnumerable<Categorium>>();

            try
            {
                // Solicita al repositorio todas las categorías.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCategoria.ListarAsync();

                // Verifica si hubo algún error al consultar la base de datos.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }

                // Guarda las categorías obtenidas dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra cualquier error ocurrido al listar categorías.
                _logger.LogError(
                    ex,
                    "Error al listar las categorías.");

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve la lista de categorías o el error.
            return resultado;
        }


        // Este método modifica una categoría existente.
        public async Task<Respuesta<Categorium>> ModificarAsync(Categorium datos)
        {
            // Crea la respuesta donde se devolverá la categoría modificada.
            var resultado = new Respuesta<Categorium>();

            try
            {
                // Busca la categoría usando su identificador.
                var categoriaActual =
                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(
                        x => x.IdCategoria == datos.IdCategoria);

                // Si no existe, no se puede modificar.
                if (categoriaActual.Data == null)
                {
                    resultado.Error =
                        "No existe la categoría que desea modificar.";

                    return resultado;
                }

                // Verifica que el nombre no esté vacío.
                if (string.IsNullOrWhiteSpace(datos.Nombre))
                {
                    resultado.Error =
                        "El nombre de la categoría es obligatorio.";

                    return resultado;
                }

                // Verifica que la familia enviada sea válida.
                if (datos.IdFamilia <= 0)
                {
                    resultado.Error =
                        "Debe seleccionar una familia válida.";

                    return resultado;
                }

                // Actualiza la familia relacionada con la categoría.
                categoriaActual.Data.IdFamilia = datos.IdFamilia;

                // Actualiza el nombre de la categoría.
                categoriaActual.Data.Nombre = datos.Nombre;

                // Actualiza la descripción.
                categoriaActual.Data.Descripcion = datos.Descripcion;

                // Actualiza el estado para indicar si está activa o inactiva.
                categoriaActual.Data.Estado = datos.Estado;

                // Envía la entidad modificada al repositorio.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCategoria.ModificarAsync(
                        categoriaActual.Data);

                // Comprueba si hubo un error durante la modificación.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }

                // Guarda la categoría modificada dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el identificador de la categoría que causó el error.
                _logger.LogError(
                    ex,
                    "Error al modificar la categoría con IdCategoria {IdCategoria}",
                    datos.IdCategoria);

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve la categoría modificada o el error.
            return resultado;
        }


        // Este método elimina una categoría.
        public async Task<Respuesta<bool>> EliminarAsync(Categorium datos)
        {
            // Crea una respuesta booleana para indicar si se eliminó correctamente.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca primero la categoría por su identificador.
                var categoria =
                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(
                        x => x.IdCategoria == datos.IdCategoria);

                // Verifica que la categoría exista antes de eliminarla.
                if (categoria.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe la categoría que desea eliminar.";

                    return resultado;
                }

                // Envía la categoría encontrada al repositorio para eliminarla.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCategoria.EliminarAsync(
                        categoria.Data);

                // Comprueba si el repositorio devolvió un error.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Data = false;
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }

                // Guarda true o false según el resultado de la eliminación.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el identificador de la categoría que se intentaba eliminar.
                _logger.LogError(
                    ex,
                    "Error al eliminar la categoría con IdCategoria {IdCategoria}",
                    datos.IdCategoria);

                // Indica que la eliminación no se completó.
                resultado.Data = false;

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado final.
            return resultado;
        }


        // Este método busca categorías utilizando el nombre recibido.
        public async Task<Respuesta<IEnumerable<Categorium>>> BuscarAsync(
            Categorium datos)
        {
            // Crea una respuesta que puede contener varias categorías.
            var resultado =
                new Respuesta<IEnumerable<Categorium>>();

            try
            {
                // Si el nombre llega vacío, utiliza una cadena vacía
                // para evitar errores al ejecutar Contains.
                var nombre = datos.Nombre ?? string.Empty;

                // Busca todas las categorías cuyo nombre contenga el texto recibido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCategoria.BuscarAsync(
                        x => x.Nombre.Contains(nombre));

                // Comprueba si hubo algún error en la consulta.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }

                // Guarda las categorías encontradas dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido durante la búsqueda.
                _logger.LogError(
                    ex,
                    "Error al buscar categorías.");

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve las categorías encontradas o el error.
            return resultado;
        }


        // Este método obtiene una categoría específica por IdCategoria.
        public async Task<Respuesta<Categorium>> ObtenerAsync(
            Categorium datos)
        {
            // Crea una respuesta para devolver una sola categoría.
            var resultado = new Respuesta<Categorium>();

            try
            {
                // Busca la categoría cuyo IdCategoria coincida con el recibido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(
                        x => x.IdCategoria == datos.IdCategoria);

                // Comprueba si la categoría fue encontrada.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Categoría no encontrada.";

                    return resultado;
                }

                // Guarda la categoría encontrada en la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra qué IdCategoria causó el error.
                _logger.LogError(
                    ex,
                    "Error al obtener la categoría con IdCategoria {IdCategoria}",
                    datos.IdCategoria);

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve la categoría encontrada o el error.
            return resultado;
        }
    }
}