using Academic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Alumnos.Commands.DeleteAlumno;

public class DeleteAlumnoCommandHandler(IAlumnoRepository repository, IDistributedCache cache)
    : IRequestHandler<DeleteAlumnoCommand, bool>
{
    public async Task<bool> Handle(DeleteAlumnoCommand request, CancellationToken cancellationToken)
    {
        // El repositorio devuelve true si @retval es 0.
        // Si @retval es > 0, se lanza una excepción y este código no se sigue ejecutando.
        bool isDeleted = await repository.DeleteAsync(request.IdActor, cancellationToken);

        if (isDeleted)
        {
            var cacheKey = $"Alumno_{request.IdActor}";
            await cache.RemoveAsync(cacheKey, cancellationToken);
        }

        return isDeleted;
    }
}