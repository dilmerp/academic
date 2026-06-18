using Academic.Application.DTOs;
using Academic.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;


namespace Academic.Application.UseCases.Alumnos.Queries.GetAllAlumnos;

public record GetAllAlumnosQuery: IRequest<IEnumerable<AlumnoDto>>, ICacheableQuery 
{
    public string CacheKey => "Alumnos_All";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}

