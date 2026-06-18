using Academic.Application.UseCases.Maestros.Queries.GetCursos;
using Academic.Application.UseCases.Maestros.Queries.GetCursosByPromocion;
using Academic.Application.UseCases.Maestros.Queries.GetEstadosCiviles;
using Academic.Application.UseCases.Maestros.Queries.GetGrupos;
using Academic.Application.UseCases.Maestros.Queries.GetPromociones;
using Academic.Application.UseCases.Maestros.Queries.GetTenores;
using Academic.Application.UseCases.Maestros.Queries.GetTiposDocumento;
using Academic.Application.UseCases.Maestros.Queries.GetPlanesPago;
using Academic.Application.UseCases.Maestros.Queries.GetBecas;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaestrosController(IMediator mediator) : ControllerBase
{
    [HttpGet("tenores")]
    public async Task<IActionResult> GetTenores(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTenoresQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("estados-civiles")]
    public async Task<IActionResult> GetEstadosCiviles(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetEstadosCivilesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("documentos-tipos")]
    public async Task<IActionResult> GetTiposDocumento(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTiposDocumentoQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("cursos")]
    public async Task<IActionResult> GetCursos(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCursosQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("promociones")]
    public async Task<IActionResult> GetPromociones(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetPromocionesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("promociones/{idPromocion:int}/grupos")]
    public async Task<IActionResult> GetGruposByPromocion(int idPromocion, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetGruposQuery(idPromocion), cancellationToken);
        return Ok(response);
    }

    [HttpGet("promociones/{idPromocion:int}/cursos")]
    public async Task<IActionResult> GetCursosByPromocion(int idPromocion, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCursosByPromocionQuery(idPromocion), cancellationToken);
        return Ok(response);
    }

    [HttpGet("planes-pago")]
    public async Task<IActionResult> GetPlanesPago(
        [FromQuery] int idPromocion,
        [FromQuery] int idGrupo,
        [FromQuery] int idSeccion,
        CancellationToken cancellationToken)
    {
        // Regla de negocio heredada: si la sección no se especifica o llega en 0, 
        // el Stored Procedure evalúa ISNULL(idseccion, -1)
        if (idSeccion == 0) idSeccion = -1;

        var response = await mediator.Send(new GetPlanesPagoQuery(idPromocion, idGrupo, idSeccion), cancellationToken);
        return Ok(response);
    }

    [HttpGet("becas")]
    public async Task<IActionResult> GetBecas(
        [FromQuery] int idActor,
        [FromQuery] int idPromocion,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetBecasQuery(idActor, idPromocion), cancellationToken);
        return Ok(response);
    }


}