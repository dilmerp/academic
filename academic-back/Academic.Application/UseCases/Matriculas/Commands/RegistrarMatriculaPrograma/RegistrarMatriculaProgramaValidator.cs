using FluentValidation;
using Academic.Domain.Interfaces;
using System.Linq;

namespace Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaPrograma;

public class RegistrarMatriculaProgramaValidator : AbstractValidator<RegistrarMatriculaProgramaCommand>
{
    public RegistrarMatriculaProgramaValidator(IValidacionMatriculaRepository validationRepo)
    {
        // 1. Validaciones de formato
        RuleFor(x => x.NumeroDocumento).NotEmpty().WithMessage("El número de documento es obligatorio.");
        RuleFor(x => x.Paterno).NotEmpty().WithMessage("El apellido paterno es obligatorio.");
        RuleFor(x => x.Nombres).NotEmpty().WithMessage("El nombre es obligatorio.");
        RuleFor(x => x.Cursos).NotNull().NotEmpty().WithMessage("Debe incluir al menos un curso.");

        // 2. Validación de Negocio: Homonimia (Documento Duplicado)
        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                // Solo validamos homonimia si es un alumno nuevo (IdAlumno == -1)
                if (cmd.IdAlumno == -1)
                {
                    var homonimo = await validationRepo.ObtenerHomónimoPorDocumentoAsync(
                        cmd.IdAlumno,
                        cmd.IdDocumento,
                        cmd.NumeroDocumento,
                        ct);

                    return homonimo == null;
                }
                return true;
            })
            .WithMessage("El documento de identidad ya se encuentra registrado a nombre de otro estudiante.");

        // 3. Validación de Negocio: Restricción de Matrícula (Evitar registrar 2 veces)
        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                // Si es un alumno nuevo, es imposible que ya esté matriculado
                if (cmd.IdAlumno == -1) return true;

                var primerCurso = cmd.Cursos.FirstOrDefault();
                if (primerCurso == null) return true;

                var resultado = await validationRepo.ValidarRestriccionPreMatriculaAsync(
                    cmd.IdAlumno,
                    cmd.IdProducto,
                    cmd.IdPromocion,
                    cmd.IdGrupo,
                    primerCurso.IdCurso,
                    primerCurso.IdSeccion, // O la propiedad equivalente en tu DTO de Programa
                    ct);

                // Si retorna > 0, ya está matriculado
                return resultado == 0;
            })
            .WithMessage("El alumno ya se encuentra matriculado en esta promoción/grupo según las reglas del sistema.");
    }
}