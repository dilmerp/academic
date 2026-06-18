using MediatR;
using System;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Profesores.Commands.UpdateProfesor;

public class UpdateProfesorCommand : IRequest<bool>
{
    public int IdActor { get; set; }
    public string Usuario { get; set; }
    public string ApellidoPaterno { get; set; }
    public string ApellidoMaterno { get; set; }
    public string Nombres { get; set; }
    public int IdTenor { get; set; }
    public bool Genero { get; set; }
    public int IdCivil { get; set; }
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
    public string Estado { get; set; }

    // Capturamos cualquier ID de auditoría que envíe el Frontend
    public int UsuarioModificacionId { get; set; }
    public int UsuarioCreacionId { get; set; }

    // Las listas ahora tienen un instanciamiento seguro por defecto
    public List<UpdateProfesorCursoCommand> Cursos { get; set; } = new List<UpdateProfesorCursoCommand>();
    public UpdateProfesorDisponibilidadCommand Disponibilidad { get; set; }
}

public class UpdateProfesorCursoCommand
{
    public int IdCurso { get; set; }
    public decimal Tarifa { get; set; }
}

public class UpdateProfesorDisponibilidadCommand
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public List<UpdateProfesorHorarioCommand> Horarios { get; set; } = new List<UpdateProfesorHorarioCommand>();
}

public class UpdateProfesorHorarioCommand
{
    public int IdHora { get; set; }
    public int IdDia { get; set; }
}