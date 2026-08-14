using Microsoft.AspNetCore.Diagnostics; // Permite manejar excepciones de forma global en la aplicación.
using Microsoft.AspNetCore.Mvc; // Permite utilizar ProblemDetails para crear respuestas de error.
using Microsoft.EntityFrameworkCore; // Permite identificar errores relacionados con Entity Framework.

namespace TiendaOnline.API.Manejadores;

// Clase encargada de manejar las excepciones que ocurren en la API.
public class ManejadorGlobalExcepciones : IExceptionHandler
{
    // Guarda el objeto utilizado para registrar los errores en los logs.
    private readonly ILogger<ManejadorGlobalExcepciones> _logger;

    // Constructor que recibe el sistema de registro de errores.
    public ManejadorGlobalExcepciones(
        ILogger<ManejadorGlobalExcepciones> logger)
    {
        // Guarda el logger recibido para utilizarlo posteriormente.
        _logger = logger;
    }

    // Método encargado de capturar y procesar las excepciones de la aplicación.
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Registra el error ocurrido junto con el mensaje de la excepción.
        _logger.LogError(
            exception,
            "Ocurrió un error no controlado: {Mensaje}",
            exception.Message
        );

        // Define inicialmente el código HTTP para un error interno del servidor.
        var estadoHttp = StatusCodes.Status500InternalServerError;

        // Define el título utilizado para el error.
        var titulo = "Error interno del servidor";

        // Define el mensaje que se mostrará al usuario.
        var mensaje = "Ocurrió un error inesperado.";

        // Revisa el tipo de excepción para determinar la respuesta adecuada.
        switch (exception)
        {
            // Se ejecuta cuando no se encuentra un registro solicitado.
            case KeyNotFoundException:
                // Establece el código HTTP 404.
                estadoHttp = StatusCodes.Status404NotFound;

                // Define el título correspondiente al error.
                titulo = "Registro no encontrado";

                // Utiliza el mensaje de la excepción.
                mensaje = exception.Message;

                // Finaliza este caso.
                break;

            // Se ejecuta cuando se reciben argumentos o datos incorrectos.
            case ArgumentException:
                // Establece el código HTTP 400.
                estadoHttp = StatusCodes.Status400BadRequest;

                // Define el título correspondiente al error.
                titulo = "Datos inválidos";

                // Utiliza el mensaje de la excepción.
                mensaje = exception.Message;

                // Finaliza este caso.
                break;

            // Se ejecuta cuando una operación no puede realizarse en el estado actual.
            case InvalidOperationException:
                // Establece el código HTTP 409.
                estadoHttp = StatusCodes.Status409Conflict;

                // Define el título correspondiente al error.
                titulo = "Operación no permitida";

                // Utiliza el mensaje de la excepción.
                mensaje = exception.Message;

                // Finaliza este caso.
                break;

            // Se ejecuta cuando el usuario no tiene autorización para realizar una acción.
            case UnauthorizedAccessException:
                // Establece el código HTTP 403.
                estadoHttp = StatusCodes.Status403Forbidden;

                // Define el título correspondiente al error.
                titulo = "Acceso denegado";

                // Utiliza el mensaje de la excepción.
                mensaje = exception.Message;

                // Finaliza este caso.
                break;

            // Se ejecuta cuando ocurre un error al actualizar la base de datos.
            case DbUpdateException:
                // Establece el código HTTP 409.
                estadoHttp = StatusCodes.Status409Conflict;

                // Define el título correspondiente al error.
                titulo = "Error al guardar en la base de datos";

                // Define un mensaje general para informar sobre el problema.
                mensaje =
                    "No se pudo guardar la información. Revise datos duplicados o relaciones existentes.";

                // Finaliza este caso.
                break;
        }

        // Establece el código HTTP que será enviado al cliente.
        httpContext.Response.StatusCode = estadoHttp;

        // Crea un objeto con la información detallada del problema.
        var respuesta = new ProblemDetails
        {
            // Guarda el código HTTP del error.
            Status = estadoHttp,

            // Guarda el título del error.
            Title = titulo,

            // Guarda la descripción del problema.
            Detail = mensaje,

            // Guarda la dirección del recurso donde ocurrió el error.
            Instance = httpContext.Request.Path
        };

        // Agrega la fecha y hora en que ocurrió el error.
        respuesta.Extensions["fecha"] = DateTime.UtcNow;

        // Agrega el método HTTP utilizado en la solicitud.
        respuesta.Extensions["metodo"] = httpContext.Request.Method;

        // Envía la respuesta de error en formato JSON al cliente.
        await httpContext.Response.WriteAsJsonAsync(
            respuesta,
            cancellationToken
        );

        // Indica que la excepción ya fue manejada correctamente.
        return true;
    }
}