using Academic.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Auth.Commands.ActualizarClave;

public class ActualizarClaveCommandHandler(IUsuarioRepository usuarioRepository)
    : IRequestHandler<ActualizarClaveCommand, bool>
{
    public async Task<bool> Handle(ActualizarClaveCommand command, CancellationToken cancellationToken)
    {
        // 1. Validar la existencia del usuario
        var usuario = await usuarioRepository.GetByLoginAsync(command.Request.Login);

        if (usuario == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o inactivo.");
        }

        // 2. Asignar la nueva contraseña (el SP se encarga del requierecambioclave = false)
        usuario.clave = command.Request.NuevaClave;

        // 3. Invocación al repositorio transaccional propagando el token de cancelación
        return await usuarioRepository.UpdateAsync(usuario, cancellationToken);
    }
}