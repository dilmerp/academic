using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Domain.Interfaces;

public interface IMatriculaCarreraRepository
{
    // Método transaccional para la nueva matrícula masiva
    Task<bool> ActualizarDistribucionAsync(
        int idPromocion,
        int idGrupo,
        int idTipo,
        int idSubTipo,
        int idProducto,
        IEnumerable<int> alumnoIds,
        IEnumerable<int> cursoIds,
        int usuarioModificacion,
        CancellationToken cancellationToken
    );

    // Método de lectura para alumnos matriculados por promoción
    Task<IEnumerable<dynamic>> GetMatriculadosByPromocionAsync(
        int idPromocion,
        string tipoFiltro,
        CancellationToken cancellationToken
    );

    // Método de lectura para el historial del alumno
    Task<IEnumerable<dynamic>> GetMatriculasByAlumnoIdAsync(
        int idAlumno,
        CancellationToken cancellationToken
    );
}