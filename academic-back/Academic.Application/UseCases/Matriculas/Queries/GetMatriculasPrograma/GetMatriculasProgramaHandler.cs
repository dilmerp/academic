using Academic.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Matriculas.Queries.GetMatriculasPrograma;

public class GetMatriculasProgramaHandler(IMatriculaProgramaRepository matriculaProgramaRepository)
    : IRequestHandler<GetMatriculasProgramaQuery, IEnumerable<object>>
{
    public async Task<IEnumerable<object>> Handle(GetMatriculasProgramaQuery request, CancellationToken cancellationToken)
    {
        // El repositorio devuelve dynamic, MediatR transporta dynamic, y el Controller lo serializa
        var result = await matriculaProgramaRepository.GetMatriculasProgramaAsync(cancellationToken);

        return result;
    }
}