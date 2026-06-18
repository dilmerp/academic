using Academic.Application.DTOs;
using Academic.Application.UseCases.Auth.Commands.Login;
using Academic.Application.UseCases.Auth.Commands.ActualizarClave;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Academic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await mediator.Send(new LoginCommand(request));
        return Ok(response);
    }

    [HttpPost("actualizar-clave")]
    public async Task<IActionResult> ActualizarClave([FromBody] ActualizarClaveRequestDto request)
    {
        try
        {
            var result = await mediator.Send(new ActualizarClaveCommand(request));

            Console.WriteLine($"ActualizarClave -> Login: {request.Login}, NuevaClave: {request.NuevaClave}, Resultado: {result}");
            if (result)
            {
                return Ok(new { mensaje = "Contraseña actualizada correctamente." });
            }
            return BadRequest("No se pudo actualizar la contraseña.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en ActualizarClave: {ex.Message}");
            return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
        }
    }
}