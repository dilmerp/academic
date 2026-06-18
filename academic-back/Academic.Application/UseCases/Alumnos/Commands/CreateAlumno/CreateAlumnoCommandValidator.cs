using FluentValidation;

namespace Academic.Application.UseCases.Alumnos.Commands.CreateAlumno;

public class CreateAlumnoCommandValidator : AbstractValidator<CreateAlumnoCommand>
{
    public CreateAlumnoCommandValidator()
    {
        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.Paterno)
            .NotEmpty().WithMessage("El apellido paterno es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido paterno no puede exceder los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El formato del email no es válido.");

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty().WithMessage("El número de documento es obligatorio.")
            .MinimumLength(8).WithMessage("El documento debe tener al menos 8 caracteres.");

        RuleFor(x => x.UsuarioCreacionId)
            .GreaterThan(0).WithMessage("El ID del usuario de creación es inválido y debe ser mayor a cero.");

        RuleFor(x => x.FechaNacimiento)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.");
    }
}