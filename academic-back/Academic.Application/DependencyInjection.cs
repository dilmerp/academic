using Academic.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Academic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Registramos MediatR y sus Behaviors
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);

            // El orden es vital: Primero Validamos, luego Cacheamos
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
        });

        // Escanea todo el ensamblado buscando clases que hereden de AbstractValidator
        // y las registra automáticamente en el contenedor de dependencias
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}