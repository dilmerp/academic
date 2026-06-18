using Academic.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Matriculas.Queries.GetMatriculasByAlumno;

public class GetMatriculasByAlumnoHandler(IMatriculaCarreraRepository matriculaRepository)
    : IRequestHandler<GetMatriculasByAlumnoQuery, IEnumerable<object>>
{
    public async Task<IEnumerable<object>> Handle(GetMatriculasByAlumnoQuery request, CancellationToken cancellationToken)
    {
        // Se añade el cancellationToken aquí para que coincida con la Interfaz
        var result = await matriculaRepository.GetMatriculasByAlumnoIdAsync(
            request.IdAlumno,
            cancellationToken
        );

        return result;
    }
}