using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Application.UseCases.Matriculas.Commands.RegistrarMatriculaPrograma;

public class RegistrarMatriculaProgramaHandler(IMatriculaProgramaRepository matriculaRepository)
    : IRequestHandler<RegistrarMatriculaProgramaCommand, string>
{
    public async Task<string> Handle(RegistrarMatriculaProgramaCommand request, CancellationToken cancellationToken)
    {
        // 1. Validaciones preventivas
        if (request.Cursos == null || !request.Cursos.Any() || request.Cuotas == null || !request.Cuotas.Any())
        {
            return string.Empty;
        }

        // 2. Extracción y mapeo al Dominio
        var cursosInfo = request.Cursos.Select(c => new CursoMatriculaInfo(
            c.IdModulo, c.IdCurso, c.IdSeccion)).ToList();

        var cuotasInfo = request.Cuotas.Select(c => new CuotaMatriculaInfo(
            c.IdConcepto, c.IdPrecio, c.PrecioDescripcion, c.Cuota, c.Cuotas,
            c.TotalCuotas, c.EsContado, c.EsInicial, c.EsCuota, c.Precio,
            c.Vencimiento, c.EsObligatorio)).ToList();

        // 3. Delegación al repositorio
        return await matriculaRepository.RegistrarMatriculaProgramaAsync(
            request.IdAlumno, request.IdRegistro, request.AlumnoCodigo, request.IdContacto,
            request.Paterno, request.Materno, request.Nombres, request.IdTenor, request.Sexo,
            request.IdDocumento, request.NumeroDocumento, request.IdCivil, request.Direccion,
            request.Urbanizacion, request.IdPais, request.IdUbigeo, request.Telefono,
            request.Celular, request.Fax, request.Email, request.IdTipo, request.IdSubTipo,
            request.IdProducto, request.IdPromocion, request.IdGrupo, request.IdCursoBase,
            request.IdSeccionBase, request.IdMedio, request.IdBeca, request.IdPlan,
            request.IdMoneda, request.IdPeriodo, request.IdFacultad, request.IdMora,
            request.UsuarioId,
            cursosInfo,
            cuotasInfo,
            cancellationToken
        );
    }
}