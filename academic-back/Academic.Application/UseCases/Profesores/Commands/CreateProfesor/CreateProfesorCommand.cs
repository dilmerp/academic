using System;
using System.Collections.Generic;
using MediatR;

namespace Academic.Application.UseCases.Profesores.Commands.CreateProfesor;

// Definición de la respuesta esperada por el Handler
public record CreateProfesorResponse(int IdActor, string Codigo);

public class CreateProfesorCommand : IRequest<CreateProfesorResponse>
{
    // --- Datos Principales ---
    public string Usuario { get; set; }
    public string Contrasena { get; set; }
    public string ApellidoPaterno { get; set; }
    public string ApellidoMaterno { get; set; }
    public string Nombres { get; set; }
    public int IdTenor { get; set; }
    public bool Genero { get; set; }
    public int IdCivil { get; set; }

    // --- Contacto e Identidad ---
    public string Email { get; set; }
    public string EmailOpcional { get; set; }
    public string EmailAdicional { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public int? IdPaisNacimiento { get; set; }
    public string IdUbigeoNacimiento { get; set; }
    public string IdUbigeoNacimientoOtro { get; set; }
    public string Telefono { get; set; }
    public string Celular { get; set; }
    public string TelefonoReferencial { get; set; }
    public string Direccion { get; set; }
    public string Urbanizacion { get; set; }
    public string DireccionReferencia { get; set; }
    public int? IdPais { get; set; }
    public string IdUbigeo { get; set; }
    public string IdUbigeoResidenciaOtro { get; set; }
    public int? IdNacionalidad { get; set; }
    public string CodigoPostal { get; set; }
    public int? IdDocumento { get; set; }
    public string NumeroDocumento { get; set; }
    public string Ruc { get; set; }

    // --- Datos de Sistema ---
    public string Estado { get; set; }
    public int? IdPeriodo { get; set; }
    public int UsuarioCreacionId { get; set; }

    // ==========================================================
    // NUEVAS PROPIEDADES PARA EL FLUJO TRANSACCIONAL (Paso 4 y 5)
    // ==========================================================
    public List<ProfesorCursoDto> Cursos { get; set; }
    public DisponibilidadDto Disponibilidad { get; set; }
}

// --- CLASES DTO AUXILIARES ---

public class ProfesorCursoDto
{
    public int IdCurso { get; set; }
    public double Tarifa { get; set; }
}

public class DisponibilidadDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public List<HorarioDetalleDto> Horarios { get; set; }
}

public class HorarioDetalleDto
{
    public int IdHora { get; set; }
    public int IdDia { get; set; }
}