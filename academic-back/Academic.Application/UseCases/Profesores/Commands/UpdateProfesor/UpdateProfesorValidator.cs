using FluentValidation;

namespace Academic.Application.UseCases.Profesores.Commands.UpdateProfesor;

public class UpdateProfesorValidator : AbstractValidator<UpdateProfesorCommand>
{
    public UpdateProfesorValidator()
    {
        RuleFor(x => x.IdActor).GreaterThan(0);
        RuleFor(x => x.ApellidoPaterno).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Usuario).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Estado).NotEmpty().Length(1);
    }
}