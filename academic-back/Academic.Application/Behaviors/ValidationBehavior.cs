using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Academic.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            // Si no hay validadores para este Request, continuamos el flujo normalmente
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Ejecutamos todos los validadores asíncronamente
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Recolectamos todos los errores
        var failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
        {
            // Si hay errores, cortamos el flujo lanzando la excepción de FluentValidation
            throw new ValidationException(failures);
        }

        // Si todo está correcto, pasamos al siguiente paso del Pipeline (o al Handler)
        return await next();
    }
}