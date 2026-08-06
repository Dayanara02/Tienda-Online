using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TiendaOnline.API.Manejadores;

public class ManejadorGlobalExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorGlobalExcepciones> _logger;

    public ManejadorGlobalExcepciones(
        ILogger<ManejadorGlobalExcepciones> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Ocurrió un error no controlado: {Mensaje}",
            exception.Message
        );

        var estadoHttp = StatusCodes.Status500InternalServerError;
        var titulo = "Error interno del servidor";
        var mensaje = "Ocurrió un error inesperado.";

        switch (exception)
        {
            case KeyNotFoundException:
                estadoHttp = StatusCodes.Status404NotFound;
                titulo = "Registro no encontrado";
                mensaje = exception.Message;
                break;

            case ArgumentException:
                estadoHttp = StatusCodes.Status400BadRequest;
                titulo = "Datos inválidos";
                mensaje = exception.Message;
                break;

            case InvalidOperationException:
                estadoHttp = StatusCodes.Status409Conflict;
                titulo = "Operación no permitida";
                mensaje = exception.Message;
                break;

            case UnauthorizedAccessException:
                estadoHttp = StatusCodes.Status403Forbidden;
                titulo = "Acceso denegado";
                mensaje = exception.Message;
                break;

            case DbUpdateException:
                estadoHttp = StatusCodes.Status409Conflict;
                titulo = "Error al guardar en la base de datos";
                mensaje =
                    "No se pudo guardar la información. Revise datos duplicados o relaciones existentes.";
                break;
        }

        httpContext.Response.StatusCode = estadoHttp;

        var respuesta = new ProblemDetails
        {
            Status = estadoHttp,
            Title = titulo,
            Detail = mensaje,
            Instance = httpContext.Request.Path
        };

        respuesta.Extensions["fecha"] = DateTime.UtcNow;
        respuesta.Extensions["metodo"] = httpContext.Request.Method;

        await httpContext.Response.WriteAsJsonAsync(
            respuesta,
            cancellationToken
        );

        return true;
    }
}