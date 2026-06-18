using Academic.Application.UseCases.Profesores.Commands.CreateProfesor;
using Academic.Application.UseCases.Profesores.Commands.DeleteProfesor;
using Academic.Application.UseCases.Profesores.Commands.UpdateProfesor;
using Academic.Application.UseCases.Profesores.Queries.GetAllProfesores;
using Academic.Application.UseCases.Profesores.Queries.GetProfesorById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfesoresController(IMediator mediator) : ControllerBase
{
    // =========================================================================
    // ENDPOINT: Obtiene el listado completo procesado por el SP
    // =========================================================================
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllProfesoresQuery();
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // =========================================================================
    // ENDPOINT: Obtiene un profesor específico por su ID
    // =========================================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetProfesorByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "No encontrado",
                Detail = $"No se encontró el profesor con ID {id}.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(result);
    }

    // =========================================================================
    // ENDPOINT: Crea un profesor de forma transaccional (Incluye Cursos y Horarios)
    // =========================================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProfesorCommand command, CancellationToken cancellationToken)
    {
        // ASP.NET Core automáticamente mapeará las listas de Cursos y Disponibilidad
        // enviadas desde Angular hacia las propiedades del command.
        var result = await mediator.Send(command, cancellationToken);

        // Ahora que tenemos el GET general, en el futuro podrías usar CreatedAtAction
        return Ok(result);
    }

    // =========================================================================
    // ENDPOINT: Actualiza los datos de un profesor existente
    // =========================================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProfesorCommand command, CancellationToken cancellationToken)
    {
        if (id != command.IdActor)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Error de consistencia",
                Detail = "El ID de la ruta HTTP no coincide con el ID del cuerpo de la petición."
            });
        }

        var result = await mediator.Send(command, cancellationToken);

        if (!result)
        {
            return NotFound(new ProblemDetails
            {
                Title = "No encontrado",
                Detail = $"No se pudo encontrar o actualizar el profesor con ID {id}."
            });
        }

        return NoContent(); // HTTP 204: Éxito sin contenido de retorno
    }

    // =========================================================================
    // ENDPOINT: Elimina a un profesor
    // =========================================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteProfesorCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (!result)
        {
            return NotFound(new ProblemDetails
            {
                Title = "No encontrado",
                Detail = $"No se encontró el profesor con ID {id} para eliminar."
            });
        }

        return NoContent(); // HTTP 204: Estándar REST para una eliminación exitosa sin contenido de retorno
    }
}