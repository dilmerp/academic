using System.Threading;
using System.Threading.Tasks;
using Academic.Application.DTOs;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Academic.Application.UseCases.Alumnos.Commands.CreateAlumno;

public class CreateAlumnoCommandHandler(
    IAlumnoRepository repository,
    IDistributedCache cache) // Inyectamos el servicio de caché
    : IRequestHandler<CreateAlumnoCommand, AlumnoCreadoDto>
{
    public async Task<AlumnoCreadoDto> Handle(CreateAlumnoCommand request, CancellationToken cancellationToken)
    {
        // Mapeamos los datos base a nuestra entidad de dominio
        var alumnoToCreate = new Alumno
        {
            Nombres = request.Nombres,
            ApellidoPaterno = request.Paterno,
            ApellidoMaterno = request.Materno,
            Documento = request.NumeroDocumento,
            Email = request.Email
        };

        // Delegamos la persistencia al repositorio
        var result = await repository.CreateAsync(
            alumnoToCreate, request.Usuario, request.Contrasena, request.IdTenor, request.Genero,
            request.IdCivil, request.EmailOpcional, request.FechaNacimiento, request.IdPaisNacimiento,
            request.IdUbigeoNacimiento, request.IdUbigeoNacimientoOtro, request.Telefono, request.Celular,
            request.TelefonoReferencial, request.Direccion, request.Urbanizacion, request.DireccionReferencia,
            request.IdPais, request.IdUbigeo, request.IdUbigeoResidenciaOtro, request.IdNacionalidad,
            request.CodigoPostal, request.IdDocumento, request.Ruc, request.Trabaja, request.Estado,
            request.IdPeriodo, request.UsuarioCreacionId, cancellationToken);

        // ===========================================================================
        // INVALIDACIÓN DE CACHÉ
        // Si existe un Query que lista todos los alumnos (ej. Alumnos_All), 
        // debemos eliminar la llave para que el listado se refresque.
        // ===========================================================================
        await cache.RemoveAsync("Alumnos_All", cancellationToken);

        return new AlumnoCreadoDto(result.IdActor, result.Codigo);
    }
}