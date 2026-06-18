using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Maestros.Queries.GetGrupos;

public class GetGruposHandler(IMaestroRepository maestroRepository) : IRequestHandler<GetGruposQuery, IEnumerable<GrupoPromocionItem>>
{
    public async Task<IEnumerable<GrupoPromocionItem>> Handle(GetGruposQuery request, CancellationToken cancellationToken)
    {
        return await maestroRepository.GetGruposByPromocionAsync(request.IdPromocion, cancellationToken);
    }
}