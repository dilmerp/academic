namespace Academic.Application.DTOs;

public record AuthResponseDto(
    string Token,
    string NombreCompleto,
    string Login,
    bool RequiereCambioClave
);