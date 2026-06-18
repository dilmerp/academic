using System;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;

namespace Academic.Domain.Interfaces;

public interface IAlumnoRepository
{
    Task<IEnumerable<AlumnoInfo>> GetAllAsync(CancellationToken cancellationToken);
    Task<AlumnoInfo?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> IsAlumnoAsync(int id, CancellationToken cancellationToken);

    Task<(int IdActor, string Codigo)> CreateAsync(Alumno actorData, string usuarioCreacion, string contrasena, int idTenor, bool genero, int idCivil, string emailOpcional, DateTime fechaNacimiento, int? idPaisNacimiento, string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular, string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia, int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad, string codigoPostal, int? idDocumento, string ruc, bool trabaja, string estado, int? idPeriodo, int usuarioCreacionId, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(int idActor, Alumno actorData, string usuario, int idTenor, bool genero, int idCivil, string emailOpcional, DateTime fechaNacimiento, int? idPaisNacimiento, string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular, string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia, int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad, string codigoPostal, int? idDocumento, string ruc, bool trabaja, string estado, int usuarioModificacionId, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<AlumnoInfo>> SearchAlumnosAsync(string termino, CancellationToken cancellationToken);
}