using FluentValidation;

namespace Academic.Application.UseCases.Auth.Commands.ActualizarClave;

public class ActualizarClaveValidator : AbstractValidator<ActualizarClaveCommand>
{
    public ActualizarClaveValidator()
    {
        RuleFor(v => v.Request.Login)
            .NotEmpty().WithMessage("El usuario institucional es requerido.");

        RuleFor(v => v.Request.NuevaClave)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}