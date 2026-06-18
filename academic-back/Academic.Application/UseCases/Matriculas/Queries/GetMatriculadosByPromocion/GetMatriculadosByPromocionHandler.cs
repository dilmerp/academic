using Academic.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Matriculas.Queries.GetMatriculadosByPromocion;

public class GetMatriculadosByPromocionHandler(IMatriculaCarreraRepository matriculaRepository)
    : IRequestHandler<GetMatriculadosByPromocionQuery, IEnumerable<object>>
{
    public async Task<IEnumerable<object>> Handle(GetMatriculadosByPromocionQuery request, CancellationToken cancellationToken)
    {
        var result = await matriculaRepository.GetMatriculadosByPromocionAsync(
            request.IdPromocion,
            request.TipoFiltro,
            cancellationToken
        );

        // 🔹 Garantiza que nunca sea null
        return result ?? Enumerable.Empty<object>();
    }
}