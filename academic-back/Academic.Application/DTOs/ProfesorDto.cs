using System;
using System.Collections.Generic;

namespace Academic.Application.DTOs;

public record ProfesorDto(
    int Id,
    string Codigo,
    string Usuario,
    string NombreCompleto,
    string Genero,

    // --- NUEVAS PROPIEDADES PARA EL TENOR ---
    int? IdTenor,
    string Tenor,
    // ----------------------------------------

    string EstadoCivil,
    string EmailPrincipal,
    string EmailAdicional,
    DateTime? FechaNacimiento,
    string Nacionalidad,
    string UbicacionResidencia,
    string DireccionCompleta,
    string TipoDocumento,
    string NumeroDocumento,
    string TelefonoContacto,
    string Estado,
    string AuditoriaCreacion,
    List<ProfesorCursoDto> Cursos,
    ProfesorDisponibilidadDto Disponibilidad
);

public record ProfesorCursoDto(
    int IdCurso,
    string Codigo,
    string Nombre,
    decimal Tarifa
);

public record ProfesorDisponibilidadDto(
    DateTime? FechaInicio,
    DateTime? FechaFin,
    List<ProfesorHorarioDto> Horarios
);

public record ProfesorHorarioDto(
    int IdHora,
    int IdDia
);