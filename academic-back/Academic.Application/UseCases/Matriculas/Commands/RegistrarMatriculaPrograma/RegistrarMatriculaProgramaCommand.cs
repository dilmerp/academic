using Academic.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaPrograma;

public record RegistrarMatriculaProgramaCommand(
    int IdAlumno,
    int IdRegistro,
    string AlumnoCodigo,
    int IdContacto,
    string Paterno,
    string Materno,
    string Nombres,
    int IdTenor,
    bool Sexo,
    int IdDocumento,
    string NumeroDocumento,
    int IdCivil,
    string Direccion,
    string Urbanizacion,
    int IdPais,
    string IdUbigeo,
    string Telefono,
    string Celular,
    string Fax,
    string Email,
    int IdTipo,
    int IdSubTipo,
    int IdProducto,
    int IdPromocion,
    int IdGrupo,
    int IdCursoBase,
    int IdSeccionBase,
    int IdMedio,
    int IdBeca,
    int IdPlan,
    int IdMoneda,
    int IdPeriodo,
    int IdFacultad,
    int IdMora,
    int UsuarioId,
    List<CursoMatriculaDto> Cursos,
    List<CuotaMatriculaDto> Cuotas
) : IRequest<string>;