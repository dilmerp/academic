using MediatR;

namespace Academic.Application.UseCases.Alumnos.Commands.DeleteAlumno;

public record DeleteAlumnoCommand(int IdActor) : IRequest<bool>;