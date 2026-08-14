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
    // Contiene la lógica de negocio de las proformas.
    public class ProformaLN : IProformaLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<ProformaLN> _logger;


        // Recibe las dependencias necesarias.
        public ProformaLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<ProformaLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra una nueva proforma.
        public async Task<Respuesta<Proforma>> InsertarAsync(
            Proforma datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Proforma>();

            try
            {
                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido.";

                    return resultado;
                }


                // Valida el subtotal.
                if (datos.Subtotal < 0)
                {
                    resultado.Error =
                        "El subtotal no puede ser negativo.";

                    return resultado;
                }


                // Valida el impuesto.
                if (datos.Impuesto < 0)
                {
                    resultado.Error =
                        "El impuesto no puede ser negativo.";

                    return resultado;
                }


                // Valida el descuento.
                if (datos.Descuento < 0)
                {
                    resultado.Error =
                        "El descuento no puede ser negativo.";

                    return resultado;
                }


                // Valida el total.
                if (datos.Total < 0)
                {
                    resultado.Error =
                        "El total no puede ser negativo.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaCreacion == default)
                {
                    datos.FechaCreacion = DateTime.Now;
                }


                // Valida la fecha de vencimiento si existe.
                if (datos.FechaVencimiento.HasValue &&
                    datos.FechaVencimiento.Value <
                    DateOnly.FromDateTime(datos.FechaCreacion))
                {
                    resultado.Error =
                        "La fecha de vencimiento no puede ser menor a la fecha de creación.";

                    return resultado;
                }


                // Asigna un estado inicial.
                if (string.IsNullOrWhiteSpace(datos.Estado))
                {
                    datos.Estado = "Pendiente";
                }


                // Guarda la proforma.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProforma.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la proforma registrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar una proforma para el usuario {IdUsuario}",
                    datos.IdUsuario);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todas las proformas.
        public async Task<Respuesta<IEnumerable<Proforma>>> ListarAsync()
        {
            // Crea la respuesta con varias proformas.
            var resultado =
                new Respuesta<IEnumerable<Proforma>>();

            try
            {
                // Consulta todas las proformas.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProforma.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las proformas encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar las proformas.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica una proforma existente.
        public async Task<Respuesta<Proforma>> ModificarAsync(
            Proforma datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Proforma>();

            try
            {
                // Busca la proforma.
                var proformaActual =
                    await _unidadDeTrabajo.TProforma.ObtenerEntidadAsync(
                        x => x.IdProforma ==
                             datos.IdProforma);


                // Comprueba que exista.
                if (proformaActual.Data == null)
                {
                    resultado.Error =
                        "No existe la proforma que desea modificar.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "El usuario no es válido.";

                    return resultado;
                }


                // Valida los valores monetarios.
                if (datos.Subtotal < 0 ||
                    datos.Impuesto < 0 ||
                    datos.Descuento < 0 ||
                    datos.Total < 0)
                {
                    resultado.Error =
                        "Los valores de la proforma no pueden ser negativos.";

                    return resultado;
                }


                // Valida la fecha de vencimiento.
                if (datos.FechaVencimiento.HasValue &&
                    datos.FechaVencimiento.Value <
                    DateOnly.FromDateTime(datos.FechaCreacion))
                {
                    resultado.Error =
                        "La fecha de vencimiento no puede ser menor a la fecha de creación.";

                    return resultado;
                }


                // Actualiza el usuario.
                proformaActual.Data.IdUsuario =
                    datos.IdUsuario;

                // Actualiza la dirección.
                proformaActual.Data.IdDireccion =
                    datos.IdDireccion;

                // Actualiza la fecha de creación.
                proformaActual.Data.FechaCreacion =
                    datos.FechaCreacion;

                // Actualiza la fecha de vencimiento.
                proformaActual.Data.FechaVencimiento =
                    datos.FechaVencimiento;

                // Actualiza el subtotal.
                proformaActual.Data.Subtotal =
                    datos.Subtotal;

                // Actualiza el impuesto.
                proformaActual.Data.Impuesto =
                    datos.Impuesto;

                // Actualiza el descuento.
                proformaActual.Data.Descuento =
                    datos.Descuento;

                // Actualiza el total.
                proformaActual.Data.Total =
                    datos.Total;

                // Actualiza el estado.
                proformaActual.Data.Estado =
                    datos.Estado;

                // Actualiza la ruta del PDF.
                proformaActual.Data.UrlPdf =
                    datos.UrlPdf;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProforma.ModificarAsync(
                        proformaActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la proforma modificada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar la proforma {IdProforma}",
                    datos.IdProforma);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina una proforma.
        public async Task<Respuesta<bool>> EliminarAsync(
            Proforma datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca la proforma.
                var proforma =
                    await _unidadDeTrabajo.TProforma.ObtenerEntidadAsync(
                        x => x.IdProforma ==
                             datos.IdProforma);


                // Comprueba que exista.
                if (proforma.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe la proforma que desea eliminar.";

                    return resultado;
                }


                // Elimina la proforma.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProforma.EliminarAsync(
                        proforma.Data);


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
                    "Error al eliminar la proforma {IdProforma}",
                    datos.IdProforma);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca proformas por su estado.
        public async Task<Respuesta<IEnumerable<Proforma>>> BuscarAsync(
            Proforma datos)
        {
            // Crea la respuesta con varias proformas.
            var resultado =
                new Respuesta<IEnumerable<Proforma>>();

            try
            {
                // Evita errores si el estado viene nulo.
                var estado =
                    datos.Estado ?? string.Empty;


                // Busca las proformas por estado.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProforma.BuscarAsync(
                        x => x.Estado.Contains(estado));


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las proformas encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar proformas.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene una proforma por su identificador.
        public async Task<Respuesta<Proforma>> ObtenerAsync(
            Proforma datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<Proforma>();

            try
            {
                // Busca la proforma.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TProforma.ObtenerEntidadAsync(
                        x => x.IdProforma ==
                             datos.IdProforma);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Proforma no encontrada.";

                    return resultado;
                }


                // Guarda la proforma encontrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener la proforma {IdProforma}",
                    datos.IdProforma);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}