using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Profesores.Queries.GetAllProfesores;

public class GetAllProfesoresHandler(IProfesorRepository repository)
    : IRequestHandler<GetAllProfesoresQuery, IEnumerable<ProfesorDto>>
{
    public async Task<IEnumerable<ProfesorDto>> Handle(GetAllProfesoresQuery request, CancellationToken cancellationToken)
    {
        var profesoresInfo = await repository.GetAllAsync(cancellationToken);

        if (profesoresInfo == null || !profesoresInfo.Any())
        {
            return new List<ProfesorDto>();
        }

        return profesoresInfo.Select(info => new ProfesorDto(
            Id: info.IdActor,
            Codigo: info.Codigo,
            Usuario: info.Usuario,
            NombreCompleto: info.NombreCompleto,
            Genero: info.Genero ? "Masculino" : "Femenino",

            // --- MAPEO DE LAS NUEVAS PROPIEDADES ---
            IdTenor: info.IdTenor,
            Tenor: info.Tenor ?? string.Empty,
            // ---------------------------------------

            EstadoCivil: info.CivilNombre,
            EmailPrincipal: info.Email,
            EmailAdicional: info.EmailAdicional,
            FechaNacimiento: DateTime.TryParse(info.FechaNacimiento, out var fn) ? fn : null,
            Nacionalidad: info.NacionalidadNombre,
            UbicacionResidencia: $"{info.DistritoNombreResidencia}, {info.PaisResidencia}".Trim(',', ' '),
            DireccionCompleta: $"{info.Direccion} {info.Urbanizacion}".Trim(),
            TipoDocumento: info.DocumentoCodigo,
            NumeroDocumento: info.NumeroDocumento,
            TelefonoContacto: !string.IsNullOrEmpty(info.Celular) ? info.Celular : info.Telefono,
            Estado: info.EstadoNombre,
            AuditoriaCreacion: info.UsuarioCreacion,
            Cursos: new List<ProfesorCursoDto>(),
            Disponibilidad: new ProfesorDisponibilidadDto(
                FechaInicio: null,
                FechaFin: null,
                Horarios: new List<ProfesorHorarioDto>()
            )
        )).ToList();
    }
}