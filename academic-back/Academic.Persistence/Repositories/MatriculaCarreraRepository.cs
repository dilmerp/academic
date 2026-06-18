using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Interfaces;
using Academic.Persistence.Data;
using Dapper;

namespace Academic.Persistence.Repositories;

public class MatriculaLecturaSeguraDto
{
    public int idregistro { get; set; }
    public int idmatricula { get; set; }
    public int idalumno { get; set; }
    public string nombrecompleto { get; set; }
    public string grupocodigo { get; set; }
    public string seccion { get; set; }
    public DateTime? matriculafecha { get; set; }
    public string matriculadopor { get; set; }
    public bool esmatricula { get; set; }
}

public class MatriculaCarreraRepository(DapperContext context) : IMatriculaCarreraRepository
{
    public async Task<bool> ActualizarDistribucionAsync(
        int idPromocion,
        int idGrupo,
        int idTipo,
        int idSubTipo,
        int idProducto,
        IEnumerable<int> alumnoIds,
        IEnumerable<int> cursoIds,
        int usuarioModificacion,
        CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        await ((DbConnection)connection).OpenAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var alumnoId in alumnoIds)
            {
                var paramRegistro = new DynamicParameters();
                paramRegistro.Add("@idalumno", alumnoId);
                paramRegistro.Add("@idtipo", idTipo);
                paramRegistro.Add("@idsubtipo", idSubTipo);
                paramRegistro.Add("@idproducto", idProducto);
                paramRegistro.Add("@idpromocion", idPromocion);
                paramRegistro.Add("@idmedio", 1);
                paramRegistro.Add("@estado", "A");
                paramRegistro.Add("@usuariocreacion", 1);
                paramRegistro.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                var cmdRegistro = new CommandDefinition(
                    "CALL up_ins_alumnoregistro(@idalumno, @idtipo, @idsubtipo, @idproducto, @idpromocion, @idmedio, @estado, @usuariocreacion, @retval)",
                    paramRegistro,
                    transaction,
                    cancellationToken: cancellationToken
                );

                await connection.ExecuteAsync(cmdRegistro);
                int idRegistroGenerado = paramRegistro.Get<int>("@retval");

                foreach (var cursoId in cursoIds)
                {
                    var paramMatricula = new DynamicParameters();
                    paramMatricula.Add("@idalumno", alumnoId);
                    paramMatricula.Add("@idregistro", idRegistroGenerado);
                    paramMatricula.Add("@idproducto", idProducto);
                    paramMatricula.Add("@idpromocion", idPromocion);
                    paramMatricula.Add("@idgrupo", idGrupo);
                    paramMatricula.Add("@idcurso", cursoId);
                    paramMatricula.Add("@idseccion", -1);
                    paramMatricula.Add("@matriculafecha", DateTime.Now);
                    paramMatricula.Add("@matriculausuario", 1);
                    paramMatricula.Add("@idmedio", 1);
                    paramMatricula.Add("@idbeca", -1);
                    paramMatricula.Add("@esmatricula", 1);
                    paramMatricula.Add("@estado", "A");
                    paramMatricula.Add("@usuariocreacion", 1);
                    paramMatricula.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                    var cmdMatricula = new CommandDefinition(
                        "CALL up_ins_alumnomatricula(@idalumno, @idregistro, @idproducto, @idpromocion, @idgrupo, @idcurso, @idseccion, @matriculafecha, @matriculausuario, @idmedio, @idbeca, @esmatricula, @estado, @usuariocreacion, @retval)",
                        paramMatricula,
                        transaction,
                        cancellationToken: cancellationToken
                    );

                    await connection.ExecuteAsync(cmdMatricula);
                }
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception($"ERROR EN TRANSACCIÓN DE MATRÍCULA: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<dynamic>> GetMatriculadosByPromocionAsync(int idPromocion, string tipoFiltro, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        await ((DbConnection)connection).OpenAsync(cancellationToken);

        string sqlQuery = $"SELECT * FROM up_sel_alumnomatricula_xmatriculados({idPromocion})";

        var command = new CommandDefinition(
            sqlQuery,
            commandType: CommandType.Text,
            commandTimeout: 120
        );

        var result = await connection.QueryAsync<MatriculaLecturaSeguraDto>(command);
        return result.Cast<dynamic>().ToList();
    }

    public async Task<IEnumerable<dynamic>> GetMatriculasByAlumnoIdAsync(int idAlumno, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        await ((DbConnection)connection).OpenAsync(cancellationToken);

        string sqlQuery = $"SELECT * FROM up_sel_alumnomatricula_xidalumno({idAlumno})";

        var command = new CommandDefinition(
            sqlQuery,
            commandType: CommandType.Text
        );

        var result = await connection.QueryAsync<MatriculaLecturaSeguraDto>(command);
        return result.Cast<dynamic>().ToList();
    }
}