using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Profesores.Commands.UpdateProfesor;

public class UpdateProfesorHandler(
    IProfesorRepository repository,
    IDistributedCache cache)
    : IRequestHandler<UpdateProfesorCommand, bool>
{
    public async Task<bool> Handle(UpdateProfesorCommand request, CancellationToken cancellationToken)
    {
        var profesor = new Profesor
        {
            ApellidoPaterno = request.ApellidoPaterno,
            ApellidoMaterno = request.ApellidoMaterno,
            Nombres = request.Nombres,
            Email = request.Email,
            Documento = request.NumeroDocumento
        };

        // Consolidar el ID de usuario de forma segura (Prioriza ModificacionId, si no existe usa CreacionId, o 1 por defecto)
        int idUsuarioAuditoria = request.UsuarioModificacionId > 0 ? request.UsuarioModificacionId : (request.UsuarioCreacionId > 0 ? request.UsuarioCreacionId : 1);

        // 1. Actualizar los datos generales de la cabecera
        var isUpdated = await repository.UpdateAsync(
            request.IdActor, profesor, request.Usuario, request.IdTenor, request.Genero,
            request.IdCivil, request.EmailOpcional, request.EmailAdicional, request.FechaNacimiento,
            request.IdPaisNacimiento, request.IdUbigeoNacimiento, request.IdUbigeoNacimientoOtro,
            request.Telefono, request.Celular, request.TelefonoReferencial, request.Direccion,
            request.Urbanizacion, request.DireccionReferencia, request.IdPais, request.IdUbigeo,
            request.IdUbigeoResidenciaOtro, request.IdNacionalidad, request.CodigoPostal,
            request.IdDocumento, request.Ruc, request.Estado, idUsuarioAuditoria,
            cancellationToken
        );

        if (!isUpdated) return false;

        // 2. Mapear Cursos a la entidad de Dominio
        var cursosInfo = request.Cursos?.Select(c => new ProfesorCursoInfo
        {
            IdCurso = c.IdCurso,
            Tarifa = c.Tarifa
        }).ToList() ?? new List<ProfesorCursoInfo>();

        // 3. Mapear Disponibilidad a la entidad de Dominio
        var disponibilidadInfo = new ProfesorDisponibilidadInfo();
        if (request.Disponibilidad != null)
        {
            disponibilidadInfo.FechaInicio = request.Disponibilidad.FechaInicio;
            disponibilidadInfo.FechaFin = request.Disponibilidad.FechaFin;
            disponibilidadInfo.Horarios = request.Disponibilidad.Horarios?.Select(h => new ProfesorHorarioInfo
            {
                IdHora = h.IdHora,
                IdDia = h.IdDia
            }).ToList() ?? new List<ProfesorHorarioInfo>();
        }

        // 4. Ejecutar la transacción en la base de datos (Elimina registros viejos y graba los nuevos)
        await repository.ReemplazarCursosYHorariosAsync(
            request.IdActor,
            cursosInfo,
            disponibilidadInfo,
            idUsuarioAuditoria,
            cancellationToken
        );

        // 5. Invalidar la caché para que el próximo GET obtenga los datos frescos de la BD
        var cacheKey = $"Profesor_{request.IdActor}";
        await cache.RemoveAsync(cacheKey, cancellationToken);

        return true;
    }
}