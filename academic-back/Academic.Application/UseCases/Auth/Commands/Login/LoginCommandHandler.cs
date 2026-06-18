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
        // 1. Buscar usuario en la base de datos usando solo el Login
        var usuario = await usuarioRepository.GetByLoginAsync(command.Request.Login);

        // 2. Validar únicamente la existencia del usuario (se ignora la clave)
        if (usuario == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado.");
        }

        // 3. Generar JWT usando el servicio de infraestructura
        var token = jwtProvider.Generate(usuario.idusuario, usuario.nombre, "ADMIN");

        // 4. Retornar el DTO usando el constructor posicional del record
        return new AuthResponseDto(
            token,
            usuario.nombre,
            usuario.login,
            usuario.requierecambioclave
        );
    }
}