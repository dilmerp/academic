using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Matriculas.Queries.GetMatriculasByAlumno;

public record GetMatriculasByAlumnoQuery(int IdAlumno) : IRequest<IEnumerable<object>>;