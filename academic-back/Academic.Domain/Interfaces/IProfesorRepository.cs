using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;

namespace Academic.Domain.Interfaces;

public interface IProfesorRepository
{
    Task<IEnumerable<ProfesorInfo>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProfesorInfo?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task ReemplazarCursosYHorariosAsync(int idActor, IEnumerable<ProfesorCursoInfo> cursos, ProfesorDisponibilidadInfo disponibilidad, int usuarioModificacionId, CancellationToken cancellationToken);

    Task<(int IdActor, string Codigo)> CreateAsync(
        Profesor profesorData, string usuario, string contrasena, int idTenor, bool genero, int idCivil,
        string emailOpcional, string emailAdicional, DateTime fechaNacimiento, int? idPaisNacimiento,
        string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular,
        string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia,
        int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad,
        string codigoPostal, int? idDocumento, string ruc, string estado, int? idPeriodo,
        int usuarioCreacionId, IDbTransaction transaction, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        int idActor, Profesor profesorData, string usuario, int idTenor, bool sexo, int idCivil,
        string emailOpcional, string emailAdicional, DateTime fechaNacimiento, int? idPaisNacimiento,
        string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular,
        string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia,
        int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad,
        string codigoPostal, int? idDocumento, string ruc, string estado, int usuarioModificacionId,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    // ------------------------------------------------------------------
    // MÉTODOS TRANSACCIONALES PARA CURSOS Y DISPONIBILIDAD
    // Usamos tipos primitivos (int, double) para no romper la Arquitectura
    // ------------------------------------------------------------------

    Task AsignarCursoAsync(
        int idActor, int idCurso, double tarifa, int usuarioCreacionId,
        IDbTransaction transaction, CancellationToken cancellationToken);

    Task<int> InsertarDisponibilidadAsync(
        int idActor, DateTime fechaInicio, DateTime fechaFin, int usuarioCreacionId,
        IDbTransaction transaction, CancellationToken cancellationToken);

    Task InsertarHorarioAsync(
        int idDisponibilidad, int idHora, int idDia,
        IDbTransaction transaction, CancellationToken cancellationToken);
}