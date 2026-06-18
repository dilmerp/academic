using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;

namespace Academic.Application.UseCases.Maestros.Queries.GetEstadosCiviles;

public record GetEstadosCivilesQuery() : IRequest<IEnumerable<MaestroItem>>;

public class GetEstadosCivilesHandler(IMaestroRepository repository) : IRequestHandler<GetEstadosCivilesQuery, IEnumerable<MaestroItem>>
{
    public async Task<IEnumerable<MaestroItem>> Handle(GetEstadosCivilesQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetEstadosCivilesAsync(cancellationToken);
    }
}