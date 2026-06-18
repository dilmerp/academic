using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Maestros.Queries.GetCursosByPromocion;

public class GetCursosByPromocionHandler(IMaestroRepository maestroRepository)
    : IRequestHandler<GetCursosByPromocionQuery, IEnumerable<MaestroItem>>
{
    public async Task<IEnumerable<MaestroItem>> Handle(GetCursosByPromocionQuery request, CancellationToken cancellationToken)
    {
        var result = await maestroRepository.GetCursosByPromocionAsync(
            request.IdPromocion,
            cancellationToken
        );

        return result;
    }
}