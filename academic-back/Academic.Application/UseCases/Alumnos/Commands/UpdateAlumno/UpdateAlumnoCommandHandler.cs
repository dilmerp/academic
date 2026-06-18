using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Alumnos.Commands.UpdateAlumno;

public class UpdateAlumnoCommandHandler(IAlumnoRepository repository, IDistributedCache cache)
    : IRequestHandler<UpdateAlumnoCommand, bool>
{
    public async Task<bool> Handle(UpdateAlumnoCommand request, CancellationToken cancellationToken)
    {
        // Construimos la entidad parcial requerida por el repositorio
        var actorData = new Alumno
        {
            ApellidoPaterno = request.Paterno,
            ApellidoMaterno = request.Materno,
            Nombres = request.Nombres,
            Email = request.Email,
            Documento = request.NumeroDocumento
        };

        var isSuccess = await repository.UpdateAsync(
            request.IdActor,
            actorData,
            request.Usuario,
            request.IdTenor,
            request.Genero,
            request.IdCivil,
            request.EmailOpcional,
            request.FechaNacimiento,
            request.IdPaisNacimiento,
            request.IdUbigeoNacimiento,
            request.IdUbigeoNacimientoOtro,
            request.Telefono,
            request.Celular,
            request.TelefonoReferencial,
            request.Direccion,
            request.Urbanizacion,
            request.DireccionReferencia,
            request.IdPais,
            request.IdUbigeo,
            request.IdUbigeoResidenciaOtro,
            request.IdNacionalidad,
            request.CodigoPostal,
            request.IdDocumento,
            request.Ruc,
            request.Trabaja,
            request.Estado,
            request.UsuarioModificacionId,
            cancellationToken
        );

        if (isSuccess)
        {
            // INVALIDACIÓN DE CACHÉ: Eliminamos el registro viejo de Redis
            var cacheKey = $"Alumno_{request.IdActor}";
            await cache.RemoveAsync(cacheKey, cancellationToken);
        }

        return isSuccess;
    }
}