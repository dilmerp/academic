using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Alumnos.Queries.GetAlumnoById;

public class GetAlumnoByIdQueryHandler(IAlumnoRepository repository)
    : IRequestHandler<GetAlumnoByIdQuery, AlumnoDto?>
{
    public async Task<AlumnoDto?> Handle(GetAlumnoByIdQuery request, CancellationToken cancellationToken)
    {
        // El repositorio ahora invoca el SP y devuelve toda la vista desnormalizada
        var info = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (info == null) return null;

        // Proyectamos el modelo de lectura de base de datos hacia el contrato de la API
        return new AlumnoDto(
            Id: info.IdActor,
            Codigo: info.Codigo,
            Usuario: info.Usuario,
            NombreCompleto: info.NombreCompleto,
            Genero: info.Genero ? "Masculino" : "Femenino", // Transformación visual ligera
            EstadoCivil: info.CivilNombre,
            EmailPrincipal: info.EMail,
            FechaNacimiento: info.FechaNacimiento,
            Nacionalidad: info.NacionalidadNombre,
            UbicacionResidencia: $"{info.DistritoNombreResidencia}, {info.PaisResidencia}".Trim(',', ' '),
            DireccionCompleta: $"{info.Direccion} {info.Urbanizacion}".Trim(),
            TipoDocumento: info.DocumentoCodigo,
            NumeroDocumento: info.NumeroDocumento,
            TelefonoContacto: !string.IsNullOrEmpty(info.Celular) ? info.Celular : info.Telefono,
            Estado: info.EstadoNombre,
            AuditoriaCreacion: info.UsuarioCreacion
        );
    }
}