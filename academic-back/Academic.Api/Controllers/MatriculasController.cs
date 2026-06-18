using Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaCarrera;
using Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaPrograma;
using Academic.Application.UseCases.Matriculas.Queries.GetMatriculadosByPromocion;
using Academic.Application.UseCases.Matriculas.Queries.GetMatriculasByAlumno;
using Academic.Application.UseCases.Matriculas.Queries.GetMatriculasPrograma;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatriculasController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtiene el historial de matrículas de un alumno específico.
    /// </summary>
    [HttpGet("alumno/{idAlumno:int}")]
    public async Task<IActionResult> GetMatriculasByAlumno(int idAlumno, CancellationToken cancellationToken)
    {
        var query = new GetMatriculasByAlumnoQuery(idAlumno);
        var result = await mediator.Send(query, cancellationToken);

        if (result == null || !result.Any())
        {
            return NotFound(new { Mensaje = $"No se encontraron matrículas para el alumno con ID {idAlumno}." });
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene la lista de alumnos matriculados filtrados por promoción.
    /// </summary>
    [HttpGet("promocion/{idPromocion:int}")]
    public async Task<IActionResult> GetMatriculadosByPromocion(
        int idPromocion,
        CancellationToken cancellationToken,
        [FromQuery] string tipoFiltro = "Todos")
    {
        Console.WriteLine($"➡️ Recibido idPromocion={idPromocion}, tipoFiltro={tipoFiltro}");

        var query = new GetMatriculadosByPromocionQuery(idPromocion, tipoFiltro);
        var result = await mediator.Send(query, cancellationToken);

        if (result == null || !result.Any())
        {
            Console.WriteLine("No se encontraron resultados");
            return NotFound(new
            {
                Mensaje = $"No se encontraron alumnos matriculados para la promoción con ID {idPromocion} aplicando el filtro '{tipoFiltro}'."
            });
        }

        Console.WriteLine($"Devueltos {result.Count()} registros");
        return Ok(result);
    }

    /// <summary>
    /// Obtiene la lista de alumnos matriculados en programas.
    /// </summary>
    [HttpGet("programa")]
    public async Task<IActionResult> GetMatriculasPrograma(CancellationToken cancellationToken)
    {
        var query = new GetMatriculasProgramaQuery();
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result ?? Enumerable.Empty<dynamic>());
    }

    /// <summary>
    /// Procesa la matrícula transaccional de uno o varios alumnos a un grupo y sus cursos (Carrera).
    /// </summary>
    [HttpPost("carrera")]
    public async Task<IActionResult> RegistrarMatriculaCarrera([FromBody] RegistrarMatriculaCarreraCommand command, CancellationToken cancellationToken)
    {
        if (command == null || command.IdAlumno <= 0 || command.IdGrupo <= 0)
        {
            return BadRequest(new { detail = "Los datos básicos del alumno y grupo son obligatorios para procesar la matrícula de carrera." });
        }

        try
        {
            var resultado = await mediator.Send(command, cancellationToken);

            if (resultado)
            {
                return Ok(new { Mensaje = "Matrícula en carrera procesada exitosamente." });
            }

            return BadRequest(new { detail = "No se pudo procesar la matrícula en carrera en la base de datos." });
        }
        catch (InvalidOperationException ex)
        {
            // Captura la excepción de negocio controlada y devuelve el mensaje descriptivo en el campo 'detail'
            return BadRequest(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            // Captura fallos técnicos inesperados de persistencia o conectividad
            return StatusCode(500, new { detail = $"Error interno en el servidor: {ex.Message}" });
        }
    }

    /// <summary>
    /// Procesa la matrícula de un alumno en un programa completo (cursos y cuotas).
    /// </summary>
    [HttpPost("programa")]
    public async Task<IActionResult> RegistrarMatriculaPrograma([FromBody] RegistrarMatriculaProgramaCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
        {
            return BadRequest(new { detail = "El payload de matrícula en programa no puede ser nulo." });
        }

        if (command.Cursos == null || !command.Cursos.Any())
        {
            return BadRequest(new { detail = "Debe enviar al menos un curso para procesar la matrícula." });
        }

        var resultado = await mediator.Send(command, cancellationToken);

        if (!string.IsNullOrEmpty(resultado))
        {
            var parts = resultado.Split('|');
            return Ok(new { AlumnoId = parts[0], Codigo = parts[1], Mensaje = "Matrícula en programa procesada exitosamente." });
        }

        return BadRequest(new { detail = "No se pudo procesar la matrícula del programa en la base de datos." });
    }
}