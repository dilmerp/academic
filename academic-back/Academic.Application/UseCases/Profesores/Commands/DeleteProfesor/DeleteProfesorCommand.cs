using MediatR;

namespace Academic.Application.UseCases.Profesores.Commands.DeleteProfesor;

public record DeleteProfesorCommand(int IdActor) : IRequest<bool>;