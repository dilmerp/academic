using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Matriculas.Queries.GetMatriculadosByPromocion;

public record GetMatriculadosByPromocionQuery(int IdPromocion, string TipoFiltro) : IRequest<IEnumerable<object>>;