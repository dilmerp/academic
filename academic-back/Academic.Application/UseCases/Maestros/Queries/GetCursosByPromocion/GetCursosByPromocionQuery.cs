using Academic.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Maestros.Queries.GetCursosByPromocion;

public record GetCursosByPromocionQuery(int IdPromocion) : IRequest<IEnumerable<MaestroItem>>;