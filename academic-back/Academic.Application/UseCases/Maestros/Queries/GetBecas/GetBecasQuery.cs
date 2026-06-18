using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;

namespace Academic.Application.UseCases.Maestros.Queries.GetBecas;

public record GetBecasQuery(int IdActor, int IdPromocion) : IRequest<IEnumerable<BecaItem>>;

public class GetBecasHandler(IMaestroRepository repository) : IRequestHandler<GetBecasQuery, IEnumerable<BecaItem>>
{
    public async Task<IEnumerable<BecaItem>> Handle(GetBecasQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetBecasAsync(request.IdActor, request.IdPromocion, cancellationToken);
    }
}