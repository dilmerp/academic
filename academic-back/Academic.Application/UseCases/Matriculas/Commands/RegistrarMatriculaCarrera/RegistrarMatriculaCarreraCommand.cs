using MediatR;

namespace Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaCarrera;

public record RegistrarMatriculaCarreraCommand(
    int IdAlumno,
    int IdPromocion,
    int IdGrupo,
    int IdPlanPago,
    int IdCurso,
    int IdBeca,
    int UsuarioModificacion
) : IRequest<bool>;