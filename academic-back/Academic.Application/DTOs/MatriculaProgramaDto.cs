using System;

namespace Academic.Application.DTOs;

public record CursoMatriculaDto(
    int IdModulo,
    int IdCurso,
    int IdSeccion
);

public record CuotaMatriculaDto(
    int IdConcepto,
    int IdPrecio,
    string PrecioDescripcion,
    int Cuota,
    int Cuotas,
    int TotalCuotas,
    bool EsContado,
    bool EsInicial,
    bool EsCuota,
    decimal Precio,
    DateTime Vencimiento,
    bool EsObligatorio
);