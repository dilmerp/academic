using Academic.Application.UseCases.Alumnos.Commands.CreateAlumno;
using Academic.Application.UseCases.Alumnos.Commands.DeleteAlumno;
using Academic.Application.UseCases.Alumnos.Commands.UpdateAlumno;
using Academic.Application.UseCases.Alumnos.Queries.GetAllAlumnos;
using Academic.Application.UseCases.Alumnos.Queries.GetAlumnoById;
using Academic.Application.UseCases.Alumnos.Queries.SearchAlumnos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class AlumnosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllAlumnosQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        // 1. Instanciamos la intención (Query)
        var query = new GetAlumnoByIdQuery(id);

        // 2. MediatR se encarga de buscar el Handler correspondiente
        var result = await mediator.Send(query, cancellationToken);

        // 3. Evaluamos el resultado
        if (result == null)
        {
            return NotFound(new { Mensaje = $"No se encontró un alumno válido con el ID {id}." });
        }

        return Ok(result);
    }


    [HttpGet("buscar")]
    public async Task<IActionResult> Search([FromQuery] string termino, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Término inválido",
                Detail = "Debe proporcionar un término de búsqueda."
            });
        }

        var query = new SearchAlumnosQuery(termino);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = "WEB,ADMIN")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlumnoCommand command, CancellationToken cancellationToken)
    {
        // Al enviar el comando, MediatR enruta automáticamente hacia CreateAlumnoCommandHandler
        var result = await mediator.Send(command, cancellationToken);

        // Retornamos un 201 Created, indicando la ruta para consultar el recurso recién creado
        return CreatedAtAction(nameof(GetById), new { id = result.IdActor }, result);
    }

    [Authorize(Roles = "WEB,ADMIN")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlumnoCommand command, CancellationToken cancellationToken)
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
                Detail = $"No se pudo encontrar o actualizar el alumno con ID {id}."
            });
        }

        return NoContent(); // HTTP 204: Estándar para una actualización exitosa sin contenido de retorno
    }


    [Authorize(Roles = "WEB,ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteAlumnoCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (!result)
        {
            return NotFound(new ProblemDetails
            {
                Title = "No encontrado",
                Detail = $"No se encontró el alumno con ID {id} para eliminar."
            });
        }

        return NoContent(); // HTTP 204: Estándar para una eliminación exitosa
    }
}