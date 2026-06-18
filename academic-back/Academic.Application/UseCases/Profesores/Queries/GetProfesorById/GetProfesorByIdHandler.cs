using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Profesores.Queries.GetProfesorById;

public class GetProfesorByIdHandler(IProfesorRepository repository)
    : IRequestHandler<GetProfesorByIdQuery, ProfesorDto?>
{
    public async Task<ProfesorDto?> Handle(GetProfesorByIdQuery request, CancellationToken cancellationToken)
    {
        var info = await repository.GetByIdAsync(request.IdActor, cancellationToken);

        if (info == null) return null;

        var cursosDto = info.Cursos?.Select(c => new ProfesorCursoDto(
            IdCurso: c.IdCurso,
            Codigo: c.Codigo,
            Nombre: c.Nombre,
            Tarifa: c.Tarifa
        )).ToList() ?? new();

        var horariosDto = info.Disponibilidad?.Horarios?.Select(h => new ProfesorHorarioDto(
            IdHora: h.IdHora,
            IdDia: h.IdDia
        )).ToList() ?? new();

        var disponibilidadDto = new ProfesorDisponibilidadDto(
            FechaInicio: info.Disponibilidad?.FechaInicio,
            FechaFin: info.Disponibilidad?.FechaFin,
            Horarios: horariosDto
        );

        return new ProfesorDto(
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
            Cursos: cursosDto,
            Disponibilidad: disponibilidadDto
        );
    }
}