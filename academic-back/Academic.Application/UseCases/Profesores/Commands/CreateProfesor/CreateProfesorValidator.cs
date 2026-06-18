using FluentValidation;

namespace Academic.Application.UseCases.Profesores.Commands.CreateProfesor;

public class CreateProfesorValidator : AbstractValidator<CreateProfesorCommand>
{
    public CreateProfesorValidator()
    {
        RuleFor(x => x.ApellidoPaterno).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Usuario).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Contrasena).NotEmpty().MaximumLength(100);
    }
}