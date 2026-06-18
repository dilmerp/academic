namespace Academic.Application.DTOs;

public record ActualizarClaveRequestDto(
    string Login,
    string NuevaClave
);