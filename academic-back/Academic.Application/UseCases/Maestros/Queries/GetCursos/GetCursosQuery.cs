using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;

namespace Academic.Application.UseCases.Maestros.Queries.GetCursos;

public record GetCursosQuery() : IRequest<IEnumerable<MaestroItem>>;

public class GetCursosHandler(IMaestroRepository repository) : IRequestHandler<GetCursosQuery, IEnumerable<MaestroItem>>
{
    public async Task<IEnumerable<MaestroItem>> Handle(GetCursosQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetCursosAsync(cancellationToken);
    }
}