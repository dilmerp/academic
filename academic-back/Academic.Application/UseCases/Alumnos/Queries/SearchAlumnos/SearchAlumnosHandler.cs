using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Alumnos.Queries.SearchAlumnos;

public class SearchAlumnosHandler(IAlumnoRepository alumnoRepository) : IRequestHandler<SearchAlumnosQuery, IEnumerable<AlumnoInfo>>
{
    public async Task<IEnumerable<AlumnoInfo>> Handle(SearchAlumnosQuery request, CancellationToken cancellationToken)
    {
        return await alumnoRepository.SearchAlumnosAsync(request.Termino, cancellationToken);
    }
}