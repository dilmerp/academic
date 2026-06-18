using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    // Definimos el código 499 porque no viene predefinido en la clase StatusCodes de .NET
    private const int Status499ClientClosedRequest = 499;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 1. Verificamos si la petición fue cancelada por el cliente (TaskCanceledException hereda de OperationCanceledException)
        if (exception is OperationCanceledException)
        {
            httpContext.Response.StatusCode = Status499ClientClosedRequest;

            var cancellationProblemDetails = new ProblemDetails
            {
                Status = Status499ClientClosedRequest,
                Title = "Petición cancelada por el cliente",
                Detail = "El usuario o el navegador cerró la conexión antes de que el servidor terminara de procesar la solicitud.",
                Instance = httpContext.Request.Path
            };

            // OJO: Usamos CancellationToken.None aquí porque el token de la petición YA está en estado "Cancelado".
            // Si le pasamos el 'cancellationToken' original, lanzaríamos una nueva excepción al intentar escribir esta respuesta.
            await httpContext.Response.WriteAsJsonAsync(cancellationProblemDetails, CancellationToken.None);

            return true;
        }

        // 2. Verificamos si la excepción viene de FluentValidation (Reglas de los Commands)
        else if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            // Agrupamos los errores por nombre de propiedad para mantener el estándar estructurado
            var errors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            // Extraemos el primer mensaje real ("El alumno ya se encuentra matriculado...") 
            // Si por alguna razón está vacío, usamos un texto de respaldo.
            string mensajeReal = validationException.Errors.FirstOrDefault()?.ErrorMessage
                                 ?? "Uno o más campos no cumplen con las reglas de negocio.";

            // Usamos la clase nativa ValidationProblemDetails
            var validationProblemDetails = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Error de validación de datos",
                Detail = mensajeReal, // <-- AQUÍ inyectamos el mensaje dinámico
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);

            // Retornamos true para decirle a .NET: "Ya manejé este error, no hagas nada más".
            return true;
        }

        // 3. Verificamos si es una excepción de Regla de Negocio / Integridad (Como el DELETE en el Repositorio)
        else if (exception is InvalidOperationException invalidOpException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            var conflictProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflicto de Integridad o Regla de Negocio",
                Detail = invalidOpException.Message,
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(conflictProblemDetails, cancellationToken);

            return true;
        }

        // 4. Si es cualquier otro error no controlado (Ej: Caída de base de datos, NullReference, etc)
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var serverErrorDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error interno del servidor",
            Detail = exception.Message, // Nota: En un entorno de Producción real, es mejor ocultar el exception.Message por seguridad.
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(serverErrorDetails, cancellationToken);

        return true;
    }
}