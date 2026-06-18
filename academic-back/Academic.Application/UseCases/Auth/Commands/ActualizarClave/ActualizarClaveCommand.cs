using MediatR;
using Academic.Application.DTOs; // Asumiendo que aquí tienes ActualizarClaveRequestDto

namespace Academic.Application.UseCases.Auth.Commands.ActualizarClave;

public record ActualizarClaveCommand(ActualizarClaveRequestDto Request) : IRequest<bool>;