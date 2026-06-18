using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Alumnos.Queries.GetAllAlumnos;

public class GetAllAlumnosHandler(IAlumnoRepository repository)
    : IRequestHandler<GetAllAlumnosQuery, IEnumerable<AlumnoDto>>
{
    public async Task<IEnumerable<AlumnoDto>> Handle(GetAllAlumnosQuery request, CancellationToken cancellationToken)
    {
        var listaInfo = await repository.GetAllAsync(cancellationToken);
        return listaInfo.Select(info => new AlumnoDto(
            Id: info.IdActor,
            Codigo: info.Codigo,
            Usuario: info.Usuario,
            NombreCompleto: info.NombreCompleto,
            Genero: info.Genero ? "Masculino" : "Femenino",
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
        ));
    }
}

