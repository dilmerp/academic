using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;

namespace Academic.Application.UseCases.Maestros.Queries.GetTiposDocumento;

public record GetTiposDocumentoQuery() : IRequest<IEnumerable<MaestroItem>>;

public class GetTiposDocumentoHandler(IMaestroRepository repository) : IRequestHandler<GetTiposDocumentoQuery, IEnumerable<MaestroItem>>
{
    public async Task<IEnumerable<MaestroItem>> Handle(GetTiposDocumentoQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetTiposDocumentoAsync(cancellationToken);
    }
}