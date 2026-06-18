using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;

namespace Academic.Application.UseCases.Maestros.Queries.GetTenores;

public record GetTenoresQuery() : IRequest<IEnumerable<MaestroItem>>;

public class GetTenoresHandler(IMaestroRepository repository) : IRequestHandler<GetTenoresQuery, IEnumerable<MaestroItem>>
{
    public async Task<IEnumerable<MaestroItem>> Handle(GetTenoresQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetTenoresAsync(cancellationToken);
    }
}