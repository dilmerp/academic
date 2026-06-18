using Academic.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Domain.Interfaces;

public interface IMaestroRepository
{
    Task<IEnumerable<MaestroItem>> GetTenoresAsync(CancellationToken cancellationToken);
    Task<IEnumerable<MaestroItem>> GetEstadosCivilesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<MaestroItem>> GetTiposDocumentoAsync(CancellationToken cancellationToken);
    Task<IEnumerable<MaestroItem>> GetCursosAsync(CancellationToken cancellationToken);
    Task<IEnumerable<MaestroItem>> GetPromocionesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<GrupoPromocionItem>> GetGruposByPromocionAsync(int idPromocion, CancellationToken cancellationToken);
    Task<IEnumerable<MaestroItem>> GetCursosByPromocionAsync(int idPromocion, CancellationToken cancellationToken);
    Task<IEnumerable<PlanPagoItem>> GetPlanesPagoAsync(int idPromocion, int idGrupo, int idSeccion, CancellationToken cancellationToken);
    Task<IEnumerable<BecaItem>> GetBecasAsync(int idActor, int idPromocion, CancellationToken cancellationToken);
}