using System;
using System.Collections.Generic;

namespace Academic.Domain.Entities;

public class ProfesorInfo
{
    public int IdActor { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public bool Genero { get; set; }

    // --- NUEVAS PROPIEDADES PARA EL TENOR ---
    public int? IdTenor { get; set; }
    public string Tenor { get; set; } = string.Empty;
    // ----------------------------------------

    public string CivilNombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmailAdicional { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;
    public string NacionalidadNombre { get; set; } = string.Empty;
    public string DistritoNombreResidencia { get; set; } = string.Empty;
    public string PaisResidencia { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Urbanizacion { get; set; } = string.Empty;
    public string DocumentoCodigo { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = string.Empty;
    public string UsuarioCreacion { get; set; } = string.Empty;

    public List<ProfesorCursoInfo> Cursos { get; set; } = new();
    public ProfesorDisponibilidadInfo Disponibilidad { get; set; } = new();
}

public class ProfesorCursoInfo
{
    public int IdCurso { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Tarifa { get; set; }
}

public class ProfesorDisponibilidadInfo
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public List<ProfesorHorarioInfo> Horarios { get; set; } = new();
}

public class ProfesorHorarioInfo
{
    public int IdHora { get; set; }
    public int IdDia { get; set; }
}