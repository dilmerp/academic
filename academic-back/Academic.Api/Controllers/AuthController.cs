using Academic.Application.DTOs;
using Academic.Application.UseCases.Auth.Commands.Login;
using Academic.Application.UseCases.Auth.Commands.ActualizarClave;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Academic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await mediator.Send(new LoginCommand(request));
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Interceptamos específicamente la validación de credenciales
            // y devolvemos un 401 Unauthorized con el mensaje exacto
            return Unauthorized(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            // Cualquier otro error de base de datos o infraestructura se mantiene como 500
            Console.WriteLine($"Error en Login: {ex.Message}");
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
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