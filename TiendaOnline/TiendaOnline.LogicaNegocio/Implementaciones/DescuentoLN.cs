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
    // Contiene la lógica de negocio de los descuentos.
    public class DescuentoLN : IDescuentoLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<DescuentoLN> _logger;


        // Recibe las dependencias necesarias.
        public DescuentoLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<DescuentoLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra un nuevo descuento.
        public async Task<Respuesta<Descuento>> InsertarAsync(
            Descuento datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Descuento>();

            try
            {
                // Valida el nombre.
                if (string.IsNullOrWhiteSpace(datos.Nombre))
                {
                    resultado.Error =
                        "Debe indicar el nombre del descuento.";

                    return resultado;
                }


                // Valida el porcentaje.
                if (datos.Porcentaje < 0 ||
                    datos.Porcentaje > 100)
                {
                    resultado.Error =
                        "El porcentaje debe estar entre 0 y 100.";

                    return resultado;
                }


                // Valida las fechas.
                if (datos.FechaFin < datos.FechaInicio)
                {
                    resultado.Error =
                        "La fecha final no puede ser menor a la inicial.";

                    return resultado;
                }


                // Guarda el descuento.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TDescuento.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el descuento registrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar un descuento.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todos los descuentos.
        public async Task<Respuesta<IEnumerable<Descuento>>> ListarAsync()
        {
            // Crea la respuesta con varios descuentos.
            var resultado =
                new Respuesta<IEnumerable<Descuento>>();

            try
            {
                // Consulta todos los descuentos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TDescuento.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los descuentos encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar los descuentos.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica un descuento existente.
        public async Task<Respuesta<Descuento>> ModificarAsync(
            Descuento datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Descuento>();

            try
            {
                // Busca el descuento.
                var descuentoActual =
                    await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(
                        x => x.IdDescuento ==
                             datos.IdDescuento);


                // Comprueba que exista.
                if (descuentoActual.Data == null)
                {
                    resultado.Error =
                        "No existe el descuento que desea modificar.";

                    return resultado;
                }


                // Valida el nombre.
                if (string.IsNullOrWhiteSpace(datos.Nombre))
                {
                    resultado.Error =
                        "Debe indicar el nombre del descuento.";

                    return resultado;
                }


                // Valida el porcentaje.
                if (datos.Porcentaje < 0 ||
                    datos.Porcentaje > 100)
                {
                    resultado.Error =
                        "El porcentaje debe estar entre 0 y 100.";

                    return resultado;
                }


                // Valida las fechas.
                if (datos.FechaFin < datos.FechaInicio)
                {
                    resultado.Error =
                        "La fecha final no puede ser menor a la inicial.";

                    return resultado;
                }


                // Actualiza el nombre.
                descuentoActual.Data.Nombre =
                    datos.Nombre;

                // Actualiza la descripción.
                descuentoActual.Data.Descripcion =
                    datos.Descripcion;

                // Actualiza el porcentaje.
                descuentoActual.Data.Porcentaje =
                    datos.Porcentaje;

                // Actualiza la fecha inicial.
                descuentoActual.Data.FechaInicio =
                    datos.FechaInicio;

                // Actualiza la fecha final.
                descuentoActual.Data.FechaFin =
                    datos.FechaFin;

                // Actualiza el estado.
                descuentoActual.Data.Estado =
                    datos.Estado;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TDescuento.ModificarAsync(
                        descuentoActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el descuento modificado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar el descuento {IdDescuento}",
                    datos.IdDescuento);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina un descuento.
        public async Task<Respuesta<bool>> EliminarAsync(
            Descuento datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca el descuento.
                var descuento =
                    await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(
                        x => x.IdDescuento ==
                             datos.IdDescuento);


                // Comprueba que exista.
                if (descuento.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe el descuento que desea eliminar.";

                    return resultado;
                }


                // Elimina el descuento.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TDescuento.EliminarAsync(
                        descuento.Data);


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
                    "Error al eliminar el descuento {IdDescuento}",
                    datos.IdDescuento);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca descuentos por nombre.
        public async Task<Respuesta<IEnumerable<Descuento>>> BuscarAsync(
            Descuento datos)
        {
            // Crea la respuesta con varios descuentos.
            var resultado =
                new Respuesta<IEnumerable<Descuento>>();

            try
            {
                // Evita errores si el nombre viene nulo.
                var nombre =
                    datos.Nombre ?? string.Empty;


                // Busca descuentos por nombre.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TDescuento.BuscarAsync(
                        x => x.Nombre.Contains(nombre));


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los descuentos encontrados.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar descuentos.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene un descuento por su identificador.
        public async Task<Respuesta<Descuento>> ObtenerAsync(
            Descuento datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Descuento>();

            try
            {
                // Busca el descuento.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(
                        x => x.IdDescuento ==
                             datos.IdDescuento);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Descuento no encontrado.";

                    return resultado;
                }


                // Guarda el descuento encontrado.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener el descuento {IdDescuento}",
                    datos.IdDescuento);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}