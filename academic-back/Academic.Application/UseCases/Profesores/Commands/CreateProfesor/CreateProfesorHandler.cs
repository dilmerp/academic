using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces; // <-- Solo usamos Domain
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Academic.Application.UseCases.Profesores.Commands.CreateProfesor;

public class CreateProfesorHandler(
    IProfesorRepository repository,
    IDbConnectionFactory connectionFactory, 
    IDistributedCache cache)
    : IRequestHandler<CreateProfesorCommand, CreateProfesorResponse>
{
    public async Task<CreateProfesorResponse> Handle(CreateProfesorCommand request, CancellationToken cancellationToken)
    {
        var profesor = new Profesor
        {
            ApellidoPaterno = request.ApellidoPaterno,
            ApellidoMaterno = request.ApellidoMaterno,
            Nombres = request.Nombres,
            Email = request.Email,
            Documento = request.NumeroDocumento
        };

        // 1. Usamos la fábrica de conexiones del dominio
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 2. Grabar Profesor Base
            var result = await repository.CreateAsync(
                profesor, request.Usuario, request.Contrasena, request.IdTenor, request.Genero,
                request.IdCivil, request.EmailOpcional, request.EmailAdicional, request.FechaNacimiento,
                request.IdPaisNacimiento, request.IdUbigeoNacimiento, request.IdUbigeoNacimientoOtro,
                request.Telefono, request.Celular, request.TelefonoReferencial, request.Direccion,
                request.Urbanizacion, request.DireccionReferencia, request.IdPais, request.IdUbigeo,
                request.IdUbigeoResidenciaOtro, request.IdNacionalidad, request.CodigoPostal,
                request.IdDocumento, request.Ruc, request.Estado, request.IdPeriodo,
                request.UsuarioCreacionId, transaction, cancellationToken
            );

            var idActor = result.IdActor;

            // 3. Grabar Cursos Asignados
            if (request.Cursos != null && request.Cursos.Count > 0)
            {
                foreach (var curso in request.Cursos)
                {
                    await repository.AsignarCursoAsync(
                        idActor, curso.IdCurso, curso.Tarifa, request.UsuarioCreacionId, transaction, cancellationToken);
                }
            }

            // 4. Grabar Disponibilidad y Horarios
            if (request.Disponibilidad != null && request.Disponibilidad.Horarios != null && request.Disponibilidad.Horarios.Count > 0)
            {
                var idDisponibilidad = await repository.InsertarDisponibilidadAsync(
                    idActor, request.Disponibilidad.FechaInicio, request.Disponibilidad.FechaFin, request.UsuarioCreacionId, transaction, cancellationToken);

                foreach (var horario in request.Disponibilidad.Horarios)
                {
                    await repository.InsertarHorarioAsync(
                        idDisponibilidad, horario.IdHora, horario.IdDia, transaction, cancellationToken);
                }
            }

            transaction.Commit();
            await cache.RemoveAsync("Profesores_All", cancellationToken);

            return new CreateProfesorResponse(result.IdActor, result.Codigo);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}