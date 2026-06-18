using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Interfaces;
using Academic.Domain.Entities;
using Academic.Persistence.Data;
using Dapper;

namespace Academic.Persistence.Repositories;

public class UsuarioRepository(DapperContext context) : IUsuarioRepository
{
    public async Task<Usuario> GetByLoginAsync(string login)
    {
        using var connection = context.CreateConnection();
        await ((DbConnection)connection).OpenAsync();

        string sqlQuery = "SELECT * FROM public.up_sel_usuario_xlogin(@p_login)";

        var command = new CommandDefinition(
            commandText: sqlQuery,
            parameters: new { p_login = login },
            commandType: CommandType.Text
        );

        return await connection.QueryFirstOrDefaultAsync<Usuario>(command);
    }

    public async Task<bool> UpdateAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var command = new CommandDefinition(
            "SELECT public.up_upd_usuario_clave2(@p_login, @p_clave)",
            new { p_login = usuario.login, p_clave = usuario.clave },
            commandType: CommandType.Text,
            commandTimeout: 120,
            cancellationToken: cancellationToken
        );
        // La función devuelve un entero (ej. 1 si se actualizó)
        var resultado = await connection.QueryFirstOrDefaultAsync<int>(command);
        return resultado > 0;
    }


    //public async Task<bool> UpdateAsync(Usuario usuario, CancellationToken cancellationToken)
    //{
    //    using var connection = context.CreateConnection();

    //    string sqlQuery = "SELECT public.up_upd_usuario_clave(@p_login, @p_clave)";

    //    var resultado = await connection.QueryFirstOrDefaultAsync<int>(
    //        sqlQuery,
    //        new
    //        {
    //            p_login = usuario.login,
    //            p_clave = usuario.clave
    //        }
    //    );

    //    return resultado > 0;
    //}

    //public async Task<bool> UpdateAsync(Usuario usuario, CancellationToken cancellationToken)
    //{
    //    using var connection = context.CreateConnection();
    //    await ((DbConnection)connection).OpenAsync(cancellationToken);

    //    // 1. Iniciamos la transacción (Le dice a Supabase que preste atención al flujo completo)
    //    using var transaction = connection.BeginTransaction();

    //    try
    //    {
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@p_login", usuario.login);
    //        parameters.Add("@p_clave", usuario.clave);
    //        // 2. Agregamos el parámetro INOUT exactamente como en alumnoregistro
    //        parameters.Add("@p_retval", dbType: DbType.Int32, direction: ParameterDirection.InputOutput, value: 0);

    //        var command = new CommandDefinition(
    //            "CALL public.up_upd_usuario_clave(@p_login, @p_clave, @p_retval)",
    //            parameters,
    //            transaction,
    //            cancellationToken: cancellationToken,commandTimeout:60
    //        );

    //        // 3. Ejecutamos el SP
    //        await connection.ExecuteAsync(command);

    //        // 4. Leemos el valor de retorno para consumirlo de la red
    //        int resultado = parameters.Get<int>("@p_retval");

    //        // 5. El Commit final obliga al pooler a cerrar la conexión y devolver respuesta HTTP
    //        transaction.Commit();
    //        return resultado > 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        transaction.Rollback();
    //        throw new Exception($"ERROR EN BD AL ACTUALIZAR CLAVE: {ex.Message}", ex);
    //    }
    //}
}