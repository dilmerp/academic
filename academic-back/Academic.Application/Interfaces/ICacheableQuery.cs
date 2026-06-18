using System;

namespace Academic.Application.Interfaces;

public interface ICacheableQuery
{
    // La llave única con la que se guardará en Redis (Ej: "Alumno_1")
    string CacheKey { get; }

    // Tiempo de vida de la caché
    TimeSpan? Expiration { get; }
}