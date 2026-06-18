using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Interfaces;
using Academic.Persistence.Data;
using Dapper;

namespace Academic.Persistence.Repositories;

public class ValidacionMatriculaRepository(DapperContext context) : IValidacionMatriculaRepository
{
    public async Task<string?> ObtenerHomónimoPorDocumentoAsync(int idActor, int idDocumento, string numeroDocumento, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var command = new CommandDefinition(
            "up_sel_actor_validadocumento",
            new { idactor = idActor, iddocumento = idDocumento, numerodocumento = numeroDocumento },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<string>(command);
    }

    public async Task<int> ValidarRestriccionPreMatriculaAsync(int idAlumno, int idProducto, int idPromocion, int idGrupo, int idCurso, int idSeccion, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var p = new DynamicParameters();

        p.Add("@idalumno", idAlumno);
        p.Add("@idproducto", idProducto);
        p.Add("@idpromocion", idPromocion);
        p.Add("@idgrupo", idGrupo);

        // Casting explícito a (int?) para garantizar que Dapper envíe un NULL tipado a SQL Server
        //p.Add("@idcurso", idCurso == -1 ? (int?)null : idCurso);
        //p.Add("@idseccion", idSeccion == -1 ? (int?)null : idSeccion);

        p.Add("@idcurso", idCurso <= 0 ? (int?)null : idCurso);
        p.Add("@idseccion", idSeccion <= 0 ? (int?)null : idSeccion);

        p.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            "up_sel_alumnoregistro_validaprematricula",
            p,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return p.Get<int>("@retval");
    }
}