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
    // Contiene la lógica de negocio de las compras a proveedores.
    public class CompraProveedorLN : ICompraProveedorLN
    {
        // Permite acceder a los repositorios.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores.
        private readonly ILogger<CompraProveedorLN> _logger;


        // Recibe las dependencias necesarias.
        public CompraProveedorLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<CompraProveedorLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }


        // Registra una nueva compra a proveedor.
        public async Task<Respuesta<CompraProveedor>> InsertarAsync(
            CompraProveedor datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<CompraProveedor>();

            try
            {
                // Valida el proveedor.
                if (datos.IdProveedor <= 0)
                {
                    resultado.Error =
                        "Debe indicar un proveedor válido.";

                    return resultado;
                }


                // Valida el usuario.
                if (datos.IdUsuario <= 0)
                {
                    resultado.Error =
                        "Debe indicar un usuario válido.";

                    return resultado;
                }


                // Valida los valores monetarios.
                if (datos.Subtotal < 0 ||
                    datos.Impuesto < 0 ||
                    datos.Total < 0)
                {
                    resultado.Error =
                        "Los valores de la compra no pueden ser negativos.";

                    return resultado;
                }


                // Asigna la fecha actual si viene vacía.
                if (datos.FechaCompra == default)
                {
                    datos.FechaCompra = DateTime.Now;
                }


                // Asigna un estado inicial.
                if (string.IsNullOrWhiteSpace(datos.Estado))
                {
                    datos.Estado = "Pendiente";
                }


                // Guarda la compra.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCompraProveedor.InsertarAsync(
                        datos);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la compra registrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al insertar una compra al proveedor {IdProveedor}",
                    datos.IdProveedor);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene todas las compras a proveedores.
        public async Task<Respuesta<IEnumerable<CompraProveedor>>> ListarAsync()
        {
            // Crea la respuesta con varias compras.
            var resultado =
                new Respuesta<IEnumerable<CompraProveedor>>();

            try
            {
                // Consulta todas las compras.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCompraProveedor.ListarAsync();


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las compras encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al listar las compras a proveedores.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Modifica una compra existente.
        public async Task<Respuesta<CompraProveedor>> ModificarAsync(
            CompraProveedor datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<CompraProveedor>();

            try
            {
                // Busca la compra por su identificador.
                var compraActual =
                    await _unidadDeTrabajo.TCompraProveedor.ObtenerEntidadAsync(
                        x => x.IdCompraProveedor ==
                             datos.IdCompraProveedor);


                // Comprueba que exista.
                if (compraActual.Data == null)
                {
                    resultado.Error =
                        "No existe la compra que desea modificar.";

                    return resultado;
                }


                // Valida el proveedor.
                if (datos.IdProveedor <= 0)
                {
                    resultado.Error =
                        "El proveedor no es válido.";

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
                    datos.Total < 0)
                {
                    resultado.Error =
                        "Los valores de la compra no pueden ser negativos.";

                    return resultado;
                }


                // Actualiza el proveedor.
                compraActual.Data.IdProveedor =
                    datos.IdProveedor;

                // Actualiza el usuario.
                compraActual.Data.IdUsuario =
                    datos.IdUsuario;

                // Actualiza la fecha.
                compraActual.Data.FechaCompra =
                    datos.FechaCompra;

                // Actualiza el subtotal.
                compraActual.Data.Subtotal =
                    datos.Subtotal;

                // Actualiza el impuesto.
                compraActual.Data.Impuesto =
                    datos.Impuesto;

                // Actualiza el total.
                compraActual.Data.Total =
                    datos.Total;

                // Actualiza el estado.
                compraActual.Data.Estado =
                    datos.Estado;


                // Guarda los cambios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCompraProveedor.ModificarAsync(
                        compraActual.Data);


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la compra modificada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al modificar la compra {IdCompraProveedor}",
                    datos.IdCompraProveedor);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Elimina una compra a proveedor.
        public async Task<Respuesta<bool>> EliminarAsync(
            CompraProveedor datos)
        {
            // Crea la respuesta booleana.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca la compra.
                var compra =
                    await _unidadDeTrabajo.TCompraProveedor.ObtenerEntidadAsync(
                        x => x.IdCompraProveedor ==
                             datos.IdCompraProveedor);


                // Comprueba que exista.
                if (compra.Data == null)
                {
                    resultado.Data = false;

                    resultado.Error =
                        "No existe la compra que desea eliminar.";

                    return resultado;
                }


                // Elimina la compra.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCompraProveedor.EliminarAsync(
                        compra.Data);


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
                    "Error al eliminar la compra {IdCompraProveedor}",
                    datos.IdCompraProveedor);

                resultado.Data = false;
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Busca compras por estado.
        public async Task<Respuesta<IEnumerable<CompraProveedor>>> BuscarAsync(
            CompraProveedor datos)
        {
            // Crea la respuesta con varias compras.
            var resultado =
                new Respuesta<IEnumerable<CompraProveedor>>();

            try
            {
                // Evita errores si el estado viene nulo.
                var estado =
                    datos.Estado ?? string.Empty;


                // Busca compras por estado.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCompraProveedor.BuscarAsync(
                        x => x.Estado.Contains(estado));


                // Comprueba si ocurrió un error.
                if (!string.IsNullOrEmpty(
                    respuestaRepositorio.Error))
                {
                    resultado.Error =
                        respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda las compras encontradas.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al buscar compras a proveedores.");

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }


        // Obtiene una compra por su identificador.
        public async Task<Respuesta<CompraProveedor>> ObtenerAsync(
            CompraProveedor datos)
        {
            // Crea la respuesta de la operación.
            var resultado = new Respuesta<CompraProveedor>();

            try
            {
                // Busca la compra.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TCompraProveedor.ObtenerEntidadAsync(
                        x => x.IdCompraProveedor ==
                             datos.IdCompraProveedor);


                // Comprueba que exista.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Compra a proveedor no encontrada.";

                    return resultado;
                }


                // Guarda la compra encontrada.
                resultado.Data =
                    respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido.
                _logger.LogError(
                    ex,
                    "Error al obtener la compra {IdCompraProveedor}",
                    datos.IdCompraProveedor);

                resultado.Error = ex.Message;
            }

            // Devuelve el resultado.
            return resultado;
        }
    }
}