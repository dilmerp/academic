using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;

namespace Academic.Domain.Interfaces;

public interface IMatriculaProgramaRepository
{
    Task<string> RegistrarMatriculaProgramaAsync(
        int idAlumno, int idRegistro, string alumnoCodigo, int idContacto, string paterno,
        string materno, string nombres, int idTenor, bool sexo, int idDocumento,
        string numeroDocumento, int idCivil, string direccion, string urbanizacion,
        int idPais, string idUbigeo, string telefono, string celular, string fax,
        string email, int idTipo, int idSubTipo, int idProducto, int idPromocion,
        int idGrupo, int idCursoBase, int idSeccionBase, int idMedio, int idBeca,
        int idPlan, int idMoneda, int idPeriodo, int idFacultad, int idMora, int usuarioId,
        IEnumerable<CursoMatriculaInfo> cursos,
        IEnumerable<CuotaMatriculaInfo> cuotas,
        CancellationToken cancellationToken);

    Task<IEnumerable<dynamic>> GetMatriculasProgramaAsync(CancellationToken cancellationToken);
}