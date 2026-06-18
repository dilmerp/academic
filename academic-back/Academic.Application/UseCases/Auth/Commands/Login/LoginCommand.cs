    using Academic.Application.DTOs;
    using MediatR;

    namespace Academic.Application.UseCases.Auth.Commands.Login;

    public record LoginCommand(LoginRequestDto Request) : IRequest<AuthResponseDto>;