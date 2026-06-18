using System.Threading;
using System.Threading.Tasks;

namespace Academic.Domain.Interfaces;

public interface IValidacionMatriculaRepository
{
    Task<string?> ObtenerHomónimoPorDocumentoAsync(
        int idActor,
        int idDocumento,
        string numeroDocumento,
        CancellationToken cancellationToken);

    Task<int> ValidarRestriccionPreMatriculaAsync(
        int idAlumno,
        int idProducto,
        int idPromocion,
        int idGrupo,
        int idCurso,
        int idSeccion,
        CancellationToken cancellationToken);
}