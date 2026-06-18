using Academic.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Maestros.Queries.GetPromociones;

// El "Record" define la intención de la consulta. Como no necesita parámetros, va vacío.
public record GetPromocionesQuery : IRequest<IEnumerable<MaestroItem>>;