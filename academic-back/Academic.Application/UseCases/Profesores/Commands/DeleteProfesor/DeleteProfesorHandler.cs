using Academic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Profesores.Commands.DeleteProfesor;

public class DeleteProfesorHandler(
    IProfesorRepository repository,
    IDistributedCache cache)
    : IRequestHandler<DeleteProfesorCommand, bool>
{
    public async Task<bool> Handle(DeleteProfesorCommand request, CancellationToken cancellationToken)
    {
        var isDeleted = await repository.DeleteAsync(request.IdActor, cancellationToken);

        if (isDeleted)
        {
            // Invalidar la caché de Redis para eliminar los datos oxidados
            var cacheKey = $"Profesor_{request.IdActor}";
            await cache.RemoveAsync(cacheKey, cancellationToken);
        }

        return isDeleted;
    }
}