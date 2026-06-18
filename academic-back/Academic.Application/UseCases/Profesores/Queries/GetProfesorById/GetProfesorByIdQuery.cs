using Academic.Application.DTOs;
using MediatR;

namespace Academic.Application.UseCases.Profesores.Queries.GetProfesorById;

public record GetProfesorByIdQuery(int IdActor) : IRequest<ProfesorDto?>;