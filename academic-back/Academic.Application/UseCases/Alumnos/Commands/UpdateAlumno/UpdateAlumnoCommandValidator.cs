using FluentValidation;

namespace Academic.Application.UseCases.Alumnos.Commands.UpdateAlumno;

public class UpdateAlumnoCommandValidator : AbstractValidator<UpdateAlumnoCommand>
{
    public UpdateAlumnoCommandValidator()
    {
        RuleFor(x => x.IdActor)
            .GreaterThan(0).WithMessage("El ID del alumno es inválido.");

        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

        RuleFor(x => x.Paterno)
            .NotEmpty().WithMessage("El apellido paterno es obligatorio.")
            .MaximumLength(20).WithMessage("El apellido paterno no puede exceder los 20 caracteres.");

        RuleFor(x => x.Materno)
            .MaximumLength(20).WithMessage("El apellido materno no puede exceder los 20 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El formato del email no es válido.")
            .MaximumLength(100).WithMessage("El email no puede exceder los 100 caracteres.");

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty().WithMessage("El número de documento es obligatorio.")
            .MinimumLength(8).WithMessage("El documento debe tener al menos 8 caracteres.")
            .MaximumLength(20).WithMessage("El documento no puede exceder los 20 caracteres.");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("El usuario es obligatorio.")
            .MaximumLength(50).WithMessage("El usuario no puede exceder los 50 caracteres.");

        RuleFor(x => x.UsuarioModificacionId)
            .GreaterThan(0).WithMessage("El ID del usuario que modifica es inválido.");
    }
}