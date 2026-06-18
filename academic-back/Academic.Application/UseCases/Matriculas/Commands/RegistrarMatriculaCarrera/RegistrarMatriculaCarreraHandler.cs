using Academic.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaCarrera;

public class RegistrarMatriculaCarreraHandler(
    IMatriculaCarreraRepository matriculaRepository,
    IMaestroRepository maestroRepository)
    : IRequestHandler<RegistrarMatriculaCarreraCommand, bool>
{
    public async Task<bool> Handle(RegistrarMatriculaCarreraCommand request, CancellationToken cancellationToken)
    {
        // 1. Extraer metadatos usando el SP de Promociones (up_sel_grupo_xidpromocion)
        var gruposDePromocion = await maestroRepository.GetGruposByPromocionAsync(request.IdPromocion, cancellationToken);

        // Extraemos el grupo específico seleccionado por el usuario en el Frontend
        var grupoDetalle = gruposDePromocion.FirstOrDefault(g => g.Id == request.IdGrupo);

        if (grupoDetalle == null)
        {
            throw new InvalidOperationException("El grupo seleccionado no existe o está inactivo en esta promoción.");
        }

        // 2. Preparar colecciones de cursos a procesar
        var alumnoIds = new List<int> { request.IdAlumno };
        var cursoIds = new List<int>();

        if (request.IdCurso > 0)
        {
            cursoIds.Add(request.IdCurso);
        }
        else
        {
            // Matricular en todos los cursos de la promoción si no se selecciona uno específico
            var cursosDelGrupo = await maestroRepository.GetCursosByPromocionAsync(request.IdPromocion, cancellationToken);
            foreach (var curso in cursosDelGrupo)
            {
                cursoIds.Add(curso.Id);
            }
        }

        // =========================================================================
        // VALIDACIÓN DE NEGOCIO INTEGRAL: Inspección de duplicados con tolerancia a nomenclatura
        // =========================================================================
        var matriculasExistentes = await matriculaRepository.GetMatriculasByAlumnoIdAsync(request.IdAlumno, cancellationToken);

        if (matriculasExistentes != null && matriculasExistentes.Any())
        {
            foreach (var matricula in matriculasExistentes)
            {
                var filaMatricula = (IDictionary<string, object>)matricula;

                // Mapeamos todas las llaves de la fila de la base de datos a minúsculas y eliminamos caracteres especiales comunes
                var llavesNormalizadas = filaMatricula.Keys.ToDictionary(
                    k => k.ToLower().Replace("_", "").Replace("ó", "o"),
                    k => k
                );

                int? idPromocionExistente = null;
                int? idCursoExistente = null;

                // Búsqueda resiliente de la promoción
                if (llavesNormalizadas.TryGetValue("idpromocion", out var llavePromo))
                {
                    idPromocionExistente = Convert.ToInt32(filaMatricula[llavePromo]);
                }

                // Búsqueda resiliente del curso
                if (llavesNormalizadas.TryGetValue("idcurso", out var llaveCurso))
                {
                    idCursoExistente = Convert.ToInt32(filaMatricula[llaveCurso]);
                }

                // Si se localizan los identificadores y coinciden con la intención de matrícula, frenamos el proceso
                if (idPromocionExistente.HasValue &&
                    idCursoExistente.HasValue &&
                    idPromocionExistente.Value == request.IdPromocion &&
                    cursoIds.Contains(idCursoExistente.Value))
                {
                    throw new InvalidOperationException("El estudiante ya registra una matrícula activa en la misma promoción y curso.");
                }
            }
        }

        // 3. Invocación al repositorio transaccional inyectando la metadata resuelta
        return await matriculaRepository.ActualizarDistribucionAsync(
            request.IdPromocion,
            request.IdGrupo,
            grupoDetalle.IdTipo,
            grupoDetalle.IdSubTipo,
            grupoDetalle.IdProducto,
            alumnoIds,
            cursoIds,
            request.UsuarioModificacion,
            cancellationToken
        );
    }
}