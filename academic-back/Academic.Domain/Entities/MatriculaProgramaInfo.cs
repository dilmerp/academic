using System;

namespace Academic.Domain.Entities;

public record CursoMatriculaInfo(
    int IdModulo,
    int IdCurso,
    int IdSeccion
);

public record CuotaMatriculaInfo(
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