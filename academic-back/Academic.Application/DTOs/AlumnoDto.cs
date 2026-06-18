namespace Academic.Application.DTOs;

public record AlumnoDto(
    int Id,
    string Codigo,
    string Usuario,
    string NombreCompleto,
    string Genero,
    string EstadoCivil,
    string EmailPrincipal,
    string FechaNacimiento,
    string Nacionalidad,
    string UbicacionResidencia,
    string DireccionCompleta,
    string TipoDocumento,
    string NumeroDocumento,
    string TelefonoContacto,
    string Estado,
    string AuditoriaCreacion
);