using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;

namespace Academic.Application.UseCases.Maestros.Queries.GetPlanesPago;

public record GetPlanesPagoQuery(int IdPromocion, int IdGrupo, int IdSeccion) : IRequest<IEnumerable<PlanPagoItem>>;

public class GetPlanesPagoHandler(IMaestroRepository repository) : IRequestHandler<GetPlanesPagoQuery, IEnumerable<PlanPagoItem>>
{
    public async Task<IEnumerable<PlanPagoItem>> Handle(GetPlanesPagoQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetPlanesPagoAsync(request.IdPromocion, request.IdGrupo, request.IdSeccion, cancellationToken);
    }
}
