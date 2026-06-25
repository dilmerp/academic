using Academic.Application.DTOs;
using Academic.Application.Interfaces;
using Academic.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Auth.Commands.Login;

public class LoginCommandHandler(IUsuarioRepository usuarioRepository, IJwtProvider jwtProvider)
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand command, CancellationToken ct)
    {
        // 1. Buscar usuario en la base de datos
        var usuario = await usuarioRepository.GetByLoginAsync(command.Request.Login);

        // 2. Validar si el usuario NO existe
        if (usuario == null)
        {
            // Usamos un mensaje genérico por seguridad
            throw new UnauthorizedAccessException("Las credenciales ingresadas son incorrectas.");
        }

        // 3. Validar la CONTRASEÑA (El paso que faltaba)
        // Nota: Asegúrate de que las propiedades coincidan con los nombres exactos de tus entidades/DTOs.
        // Si a futuro usas Hashes (ej. BCrypt), aquí usarías BCrypt.Verify() en lugar del ==
        if (usuario.clave != command.Request.Clave)
        {
            throw new UnauthorizedAccessException("Las credenciales ingresadas son incorrectas.");
        }

        // 4. Generar JWT usando el servicio de infraestructura
        var token = jwtProvider.Generate(usuario.idusuario, usuario.nombre, "ADMIN");

        // 5. Retornar el DTO usando el constructor posicional del record
        return new AuthResponseDto(
            token,
            usuario.nombre,
            usuario.login,
            usuario.requierecambioclave
        );
    }
}