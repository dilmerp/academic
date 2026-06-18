using System;
using Academic.Application.DTOs;
using Academic.Application.Interfaces;
using MediatR;

namespace Academic.Application.UseCases.Alumnos.Queries.GetAlumnoById;

public record GetAlumnoByIdQuery(int Id) : IRequest<AlumnoDto?>, ICacheableQuery
{
    // Definimos cómo se llamará la llave en Redis para este alumno específico
    public string CacheKey => $"Alumno_{Id}";

    // Le damos un tiempo de vida de 5 minutos en caché
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}