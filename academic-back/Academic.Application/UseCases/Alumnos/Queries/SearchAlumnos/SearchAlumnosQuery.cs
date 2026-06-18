using Academic.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Alumnos.Queries.SearchAlumnos;

public record SearchAlumnosQuery(string Termino) : IRequest<IEnumerable<AlumnoInfo>>;