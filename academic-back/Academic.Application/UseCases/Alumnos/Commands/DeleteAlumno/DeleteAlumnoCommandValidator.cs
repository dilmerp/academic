using FluentValidation;

namespace Academic.Application.UseCases.Alumnos.Commands.DeleteAlumno;

public class DeleteAlumnoCommandValidator : AbstractValidator<DeleteAlumnoCommand>
{
    public DeleteAlumnoCommandValidator()
    {
        RuleFor(x => x.IdActor)
            .GreaterThan(0).WithMessage("El ID del alumno a eliminar es inválido y debe ser mayor a cero.");
    }
}