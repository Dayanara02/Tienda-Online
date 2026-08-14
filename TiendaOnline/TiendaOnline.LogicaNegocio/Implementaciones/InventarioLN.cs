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
    // Contiene la lógica de negocio del inventario.
    public class InventarioLN : IInventarioLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<InventarioLN> _logger;


        // Recibe las dependencias necesarias.
        public InventarioLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<InventarioLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra un nuevo inventario.
        public async Task<Respuesta<Inventario>> InsertarAsync(
            Inventario datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Inventario>();

            try
            {
                // Valida el producto.
                if (datos.IdProducto <= 0)
                {
                    resultado.Error =
                        "Debe indicar un producto válido.";

                    return resultado;
                }


                // Valida la cantidad disponible.
                if (datos.CantidadDisponible < 0)
                {
                    resultado.Error =
                        "La cantidad disponible no puede ser negativa.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaActualizacion == default)
                {
                    datos.FechaActualizacion = DateTime.Now;
                }


                // Guarda el inventario.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TInventario.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el inventario registrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar inventario para el producto {IdProducto}",
                    datos.IdProducto);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todos los inventarios.
        public async Task<Respuesta<IEnumerable<Inventario>>> ListarAsync()
        {
            // Crea la respuesta con varios inventarios.
            var resultado =
                new Respuesta<IEnumerable<Inventario>>();

            try
            {
                // Consulta todos los inventarios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TInventario.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los inventarios encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar los inventarios.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica un inventario existente.
        public async Task<Respuesta<Inventario>> ModificarAsync(
            Inventario datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Inventario>();

            try
            {
                // Busca el inventario.
                var inventarioActual =
                    await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(
                        x => x.IdInventario ==
                             datos.IdInventario);


                // Comprueba que exista.
                if (inventarioActual.Data == null)
                {
                    resultado.Error =
                        "No existe el inventario que desea modificar.";

                    return resultado;
                }


                // Valida el producto.
                if (datos.IdProducto <= 0)
                {
                    resultado.Error =
                        "El producto no es válido.";

                    return resultado;
                }


                // Valida la cantidad.
                if (datos.CantidadDisponible < 0)
                {
                    resultado.Error =
                        "La cantidad disponible no puede ser negativa.";

                    return resultado;
                }


                // Actualiza el producto.
                inventarioActual.Data.IdProducto =
                    datos.IdProducto;

                // Actualiza la cantidad disponible.
                inventarioActual.Data.CantidadDisponible =
                    datos.CantidadDisponible;

                // Actualiza la fecha.
                inventarioActual.Data.FechaActualizacion =
                    DateTime.Now;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TInventario.ModificarAsync(
                        inventarioActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el inventario modificado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar el inventario {IdInventario}",
                    datos.IdInventario);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina un inventario.
        public async Task<Respuesta<bool>> EliminarAsync(
            Inventario datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca el inventario.
                var inventario =
                    await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(
                        x => x.IdInventario ==
                             datos.IdInventario);


                // Comprueba que exista.
                if (inventario.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe el inventario que desea eliminar.";

                    return resultado;
                }


                // Elimina el inventario.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TInventario.EliminarAsync(
                        inventario.Data);


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
                    "Error al eliminar el inventario {IdInventario}",
                    datos.IdInventario);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca inventarios por producto.
        public async Task<Respuesta<IEnumerable<Inventario>>> BuscarAsync(
            Inventario datos)
        {
            // Crea la respuesta con varios inventarios.
            var resultado =
                new Respuesta<IEnumerable<Inventario>>();

            try
            {
                // Busca por identificador del producto.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TInventario.BuscarAsync(
                        x => datos.IdProducto <= 0 ||
                             x.IdProducto == datos.IdProducto);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los inventarios encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar inventarios.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene un inventario por su identificador.
        public async Task<Respuesta<Inventario>> ObtenerAsync(
            Inventario datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Inventario>();

            try
            {
                // Busca el inventario.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(
                        x => x.IdInventario ==
                             datos.IdInventario);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Inventario no encontrado.";

                    return resultado;
                }


                // Guarda el inventario encontrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener el inventario {IdInventario}",
                    datos.IdInventario);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}
