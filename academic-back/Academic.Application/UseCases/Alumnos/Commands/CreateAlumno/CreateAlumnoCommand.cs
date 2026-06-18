using System;
using Academic.Application.DTOs;
using MediatR;

namespace Academic.Application.UseCases.Alumnos.Commands.CreateAlumno;

public record CreateAlumnoCommand(
    string Usuario,
    string Contrasena,
    string Paterno,
    string Materno,
    string Nombres,
    int IdTenor,
    bool Genero,
    int IdCivil,
    string Email,
    string EmailOpcional,
    DateTime FechaNacimiento,
    int? IdPaisNacimiento,
    string IdUbigeoNacimiento,
    string IdUbigeoNacimientoOtro,
    string Telefono,
    string Celular,
    string TelefonoReferencial,
    string Direccion,
    string Urbanizacion,
    string DireccionReferencia,
    int? IdPais,
    string IdUbigeo,
    string IdUbigeoResidenciaOtro,
    int? IdNacionalidad,
    string CodigoPostal,
    int? IdDocumento,
    string NumeroDocumento,
    string Ruc,
    bool Trabaja,
    string Estado,
    int? IdPeriodo,
    int UsuarioCreacionId
) : IRequest<AlumnoCreadoDto>;