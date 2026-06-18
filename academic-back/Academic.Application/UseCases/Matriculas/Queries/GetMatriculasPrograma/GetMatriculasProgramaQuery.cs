using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Matriculas.Queries.GetMatriculasPrograma;

public record GetMatriculasProgramaQuery() : IRequest<IEnumerable<object>>;
