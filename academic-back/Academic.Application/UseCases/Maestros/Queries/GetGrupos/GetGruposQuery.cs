using Academic.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Maestros.Queries.GetGrupos;

public record GetGruposQuery(int IdPromocion) : IRequest<IEnumerable<GrupoPromocionItem>>;