using FluentValidation;

namespace Academic.Application.UseCases.Profesores.Commands.DeleteProfesor;

public class DeleteProfesorValidator : AbstractValidator<DeleteProfesorCommand>
{
    public DeleteProfesorValidator()
    {
        RuleFor(x => x.IdActor).GreaterThan(0).WithMessage("El ID del profesor debe ser mayor a cero.");
    }
}