using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using Academic.Persistence.Data;
using Dapper;

namespace Academic.Persistence.Repositories;

public class AlumnoRepository(DapperContext context) : IAlumnoRepository
{
    //public async Task<IEnumerable<AlumnoInfo>> GetAllAsync(CancellationToken cancellationToken)
    //{
    //    using var connection = context.CreateConnection();
    //    var command = new CommandDefinition(
    //        commandText: "up_sel_actor_alumno_all",
    //        commandType: CommandType.StoredProcedure,
    //        cancellationToken: cancellationToken
    //    );
    //    return await connection.QueryAsync<AlumnoInfo>(command);
    //}

    public async Task<IEnumerable<AlumnoInfo>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        var command = new CommandDefinition(
            commandText: "SELECT * FROM public.up_sel_actor_alumno_all()",
            commandType: CommandType.Text,
            cancellationToken: cancellationToken
        );

        return await connection.QueryAsync<AlumnoInfo>(command);
    }

    //public async Task<AlumnoInfo?> GetByIdAsync(int id, CancellationToken cancellationToken)
    //{
    //    using var connection = context.CreateConnection();

    //    var command = new CommandDefinition(
    //        commandText: "up_sel_actor_xidalumno",
    //        parameters: new { idactor = id },
    //        commandType: CommandType.StoredProcedure,
    //        cancellationToken: cancellationToken
    //    );

    //    return await connection.QuerySingleOrDefaultAsync<AlumnoInfo>(command);
    //}

    public async Task<AlumnoInfo?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        var command = new CommandDefinition(
            commandText: "SELECT * FROM public.up_sel_actor_xidalumno(@idactor)",
            parameters: new { idactor = id },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken
        );

        return await connection.QuerySingleOrDefaultAsync<AlumnoInfo>(command);
    }

    //public async Task<bool> IsAlumnoAsync(int id, CancellationToken cancellationToken)
    //{
    //    const string sql = "SELECT COUNT(1) FROM dbinstituto.dbo.ActorRol WHERE IdActor = @Id AND IdRol = 3";

    //    using var connection = context.CreateConnection();
    //    var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);

    //    var count = await connection.ExecuteScalarAsync<int>(command);
    //    return count > 0;
    //}

    public async Task<bool> IsAlumnoAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        var command = new CommandDefinition(
            commandText: "fn_is_alumno",
            parameters: new { p_idactor = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<(int IdActor, string Codigo)> CreateAsync(Alumno actorData, string usuarioCreacion, string contrasena, int idTenor, bool genero, int idCivil, string emailOpcional, DateTime fechaNacimiento, int? idPaisNacimiento, string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular, string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia, int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad, string codigoPostal, int? idDocumento, string ruc, bool trabaja, string estado, int? idPeriodo, int usuarioCreacionId, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@usuario", usuarioCreacion);
        parameters.Add("@contrasena", contrasena);
        parameters.Add("@paterno", actorData.ApellidoPaterno);
        parameters.Add("@materno", actorData.ApellidoMaterno);
        parameters.Add("@nombres", actorData.Nombres);
        parameters.Add("@idtenor", idTenor);
        parameters.Add("@genero", genero);
        parameters.Add("@idcivil", idCivil);
        parameters.Add("@email", actorData.Email);
        parameters.Add("@emailopcional", emailOpcional);
        parameters.Add("@fechanacimiento", fechaNacimiento);
        parameters.Add("@idpaisnacimiento", idPaisNacimiento ?? -1);
        parameters.Add("@idubigeonacimiento", string.IsNullOrEmpty(idUbigeoNacimiento) ? "-1" : idUbigeoNacimiento);
        parameters.Add("@idubigeonacimientootro", string.IsNullOrEmpty(idUbigeoNacimientoOtro) ? "" : idUbigeoNacimientoOtro);
        parameters.Add("@telefono", telefono);
        parameters.Add("@celular", celular);
        parameters.Add("@telefonoreferencial", telefonoReferencial);
        parameters.Add("@direccion", direccion);
        parameters.Add("@urbanizacion", urbanizacion);
        parameters.Add("@direccionreferencia", direccionReferencia);
        parameters.Add("@idpais", idPais ?? -1);
        parameters.Add("@idubigeo", string.IsNullOrEmpty(idUbigeo) ? "-1" : idUbigeo);
        parameters.Add("@idubigeoresidenciaotro", string.IsNullOrEmpty(idUbigeoResidenciaOtro) ? "" : idUbigeoResidenciaOtro);
        parameters.Add("@idnacionalidad", idNacionalidad ?? -1);
        parameters.Add("@codigopostal", codigoPostal);
        parameters.Add("@iddocumento", idDocumento ?? -1);
        parameters.Add("@numerodocumento", actorData.Documento);
        parameters.Add("@ruc", ruc);
        parameters.Add("@trabaja", trabaja);
        parameters.Add("@estado", estado);
        parameters.Add("@idperiodo", idPeriodo ?? -1);
        parameters.Add("@usuariocreacion", usuarioCreacionId);

        parameters.Add("@retval", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

        using var connection = context.CreateConnection();

        var commandDefinition = new CommandDefinition(
            commandText: "up_ins_actor_xalumno",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(commandDefinition);

        string outputValue = parameters.Get<string>("@retval");

        if (!string.IsNullOrEmpty(outputValue) && outputValue.Contains('|'))
        {
            var parts = outputValue.Split('|');
            if (int.TryParse(parts[0], out int newId))
            {
                return (newId, parts[1]);
            }
        }

        throw new Exception("El Stored Procedure no devolvió el formato esperado en el parámetro @retval.");
    }

    public async Task<bool> UpdateAsync(int idActor, Alumno actorData, string usuario, int idTenor, bool genero, int idCivil, string emailOpcional, DateTime fechaNacimiento, int? idPaisNacimiento, string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular, string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia, int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad, string codigoPostal, int? idDocumento, string ruc, bool trabaja, string estado, int usuarioModificacionId, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@idactor", idActor);
        parameters.Add("@usuario", usuario);
        parameters.Add("@paterno", actorData.ApellidoPaterno);
        parameters.Add("@materno", actorData.ApellidoMaterno);
        parameters.Add("@nombres", actorData.Nombres);
        parameters.Add("@idtenor", idTenor);
        parameters.Add("@sexo", genero);
        parameters.Add("@idcivil", idCivil);
        parameters.Add("@email", actorData.Email);
        parameters.Add("@emailopcional", emailOpcional);
        parameters.Add("@fechanacimiento", fechaNacimiento);
        parameters.Add("@idpaisnacimiento", idPaisNacimiento ?? -1);
        parameters.Add("@idubigeonacimiento", string.IsNullOrEmpty(idUbigeoNacimiento) ? "-1" : idUbigeoNacimiento);
        parameters.Add("@idubigeonacimientootro", string.IsNullOrEmpty(idUbigeoNacimientoOtro) ? "" : idUbigeoNacimientoOtro);
        parameters.Add("@telefono", telefono);
        parameters.Add("@celular", celular);
        parameters.Add("@telefonoreferencial", telefonoReferencial);
        parameters.Add("@direccion", direccion);
        parameters.Add("@urbanizacion", urbanizacion);
        parameters.Add("@direccionreferencia", direccionReferencia);
        parameters.Add("@idpais", idPais ?? -1);
        parameters.Add("@idubigeo", string.IsNullOrEmpty(idUbigeo) ? "-1" : idUbigeo);
        parameters.Add("@idubigeoresidenciaotro", string.IsNullOrEmpty(idUbigeoResidenciaOtro) ? "" : idUbigeoResidenciaOtro);
        parameters.Add("@idnacionalidad", idNacionalidad ?? -1);
        parameters.Add("@codigopostal", codigoPostal);
        parameters.Add("@iddocumento", idDocumento ?? -1);
        parameters.Add("@numerodocumento", actorData.Documento);
        parameters.Add("@ruc", ruc);
        parameters.Add("@trabaja", trabaja);
        parameters.Add("@estado", estado);
        parameters.Add("@usuariomodificacion", usuarioModificacionId);

        using var connection = context.CreateConnection();

        var commandDefinition = new CommandDefinition(
            commandText: "up_upd_actor_xalumno",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.ExecuteAsync(commandDefinition);

        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@idactor", id);
        parameters.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = context.CreateConnection();

        var commandDefinition = new CommandDefinition(
            commandText: "up_del_actor_xalumno",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(commandDefinition);

        int resultValue = parameters.Get<int>("@retval");

        if (resultValue == 0) return true;

        throw new InvalidOperationException(resultValue switch
        {
            1 => "No se puede eliminar: El alumno tiene una matrícula vigente.",
            2 => "No se puede eliminar: El alumno tiene registros en el historial.",
            3 => "No se puede eliminar: El alumno tiene currícula asignada.",
            4 => "No se puede eliminar: El alumno está inscrito en cursos.",
            5 => "No se puede eliminar: El alumno ya cuenta con notas registradas.",
            6 => "No se puede eliminar: El actor tiene otros roles asignados además de alumno.",
            _ => "No se pudo eliminar el alumno debido a restricciones de integridad."
        });
    }

    public async Task<IEnumerable<AlumnoInfo>> SearchAlumnosAsync(string termino, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var parameters = new { nombrecompleto = termino };

        var command = new CommandDefinition(
            "up_sel_actor_xbuscar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new AlumnoInfo
        {
            IdActor = x.idactor,
            NombreCompleto = x.nombrecompleto ?? string.Empty,
            Codigo = x.codigo?.ToString() ?? string.Empty,
            DocumentoCodigo = x.documentocodigo?.ToString() ?? string.Empty,
            NumeroDocumento = x.numerodocumento?.ToString() ?? string.Empty,
            EMail = x.email?.ToString() ?? string.Empty
        });
    }
}