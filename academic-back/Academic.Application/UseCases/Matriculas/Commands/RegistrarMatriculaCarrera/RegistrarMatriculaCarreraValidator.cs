using FluentValidation;

namespace Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaCarrera;

public class RegistrarMatriculaCarreraValidator : AbstractValidator<RegistrarMatriculaCarreraCommand>
{
    public RegistrarMatriculaCarreraValidator()
    {
        RuleFor(v => v.IdAlumno)
            .GreaterThan(0).WithMessage("El ID del alumno es requerido y debe ser mayor a 0.");

        RuleFor(v => v.IdPromocion)
            .GreaterThan(0).WithMessage("Debe seleccionar una promoción válida.");

        RuleFor(v => v.IdGrupo)
            .GreaterThan(0).WithMessage("Debe seleccionar un grupo válido.");

        RuleFor(v => v.IdPlanPago)
            .GreaterThan(0).WithMessage("Debe seleccionar un plan de pago.");

        RuleFor(v => v.UsuarioModificacion)
            .GreaterThan(0).WithMessage("El usuario de modificación es requerido.");
    }
}