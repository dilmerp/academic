namespace Academic.Application.DTOs;

public record MatriculaResponseDto(
    int IdAlumno,
    int IdRegistro,
    int IdMatricula,
    int IdPromocion,
    int IdGrupo,
    bool EsMatricula
);