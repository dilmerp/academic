using Academic.Application.DTOs;
using Academic.Application.Interfaces; // Para ICacheableQuery
using MediatR;
using System;
using System.Collections.Generic;

namespace Academic.Application.UseCases.Profesores.Queries.GetAllProfesores;

// Implementamos ICacheableQuery para activar el CachingBehavior
public record GetAllProfesoresQuery : IRequest<IEnumerable<ProfesorDto>>, ICacheableQuery
{
    // Llave única para el listado completo de profesores
    public string CacheKey => "Profesores_All";

    // Tiempo de vida en Redis (ajusta según la frecuencia de cambio de los datos)
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}