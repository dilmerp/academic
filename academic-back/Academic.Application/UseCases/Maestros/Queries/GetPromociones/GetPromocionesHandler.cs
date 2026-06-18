using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Maestros.Queries.GetPromociones;

// El "Handler" es el que ejecuta el trabajo real yendo al repositorio
public class GetPromocionesHandler(IMaestroRepository maestroRepository) : IRequestHandler<GetPromocionesQuery, IEnumerable<MaestroItem>>
{
    public async Task<IEnumerable<MaestroItem>> Handle(GetPromocionesQuery request, CancellationToken cancellationToken)
    {
        return await maestroRepository.GetPromocionesAsync(cancellationToken);
    }
}