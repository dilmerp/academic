using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Academic.Application.DTOs;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using Academic.Persistence.Data;
using Dapper;

namespace Academic.Persistence.Repositories;

public class ProfesorRepository(DapperContext context) : IProfesorRepository
{
    public async Task<IEnumerable<ProfesorInfo>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        var command = new CommandDefinition(
            commandText: "up_sel_actor_profesor_all",
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        return await connection.QueryAsync<ProfesorInfo>(command);
    }

    public async Task<ProfesorInfo?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        var commandHeader = new CommandDefinition(
            commandText: "up_sel_actor_xidprofesor",
            parameters: new { idactor = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var info = await connection.QuerySingleOrDefaultAsync<ProfesorInfo>(commandHeader);

        if (info != null)
        {
            info.Cursos = new List<ProfesorCursoInfo>();
            info.Disponibilidad = new ProfesorDisponibilidadInfo();

            var commandCursos = new CommandDefinition(
                commandText: "up_sel_profesorcurso_xidprofesor",
                parameters: new { idactor = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            var cursos = await connection.QueryAsync<ProfesorCursoInfo>(commandCursos);
            info.Cursos = cursos.ToList();

            var commandDispo = new CommandDefinition(
                commandText: "up_sel_profesordisponibilidad_xidactor",
                parameters: new { idactor = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            var dispoData = await connection.QueryFirstOrDefaultAsync<dynamic>(commandDispo);

            if (dispoData != null)
            {
                if (DateTime.TryParse(dispoData.fechainicio?.ToString(), out DateTime fi))
                    info.Disponibilidad.FechaInicio = fi;

                if (DateTime.TryParse(dispoData.fechafin?.ToString(), out DateTime ff))
                    info.Disponibilidad.FechaFin = ff;

                int idDisponibilidad = Convert.ToInt32(dispoData.iddisponibilidad);

                var commandHorarios = new CommandDefinition(
                    commandText: "up_sel_profesorhorario_xiddisponibilidad",
                    parameters: new { iddisponibilidad = idDisponibilidad },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken
                );

                var pivotHorarios = await connection.QueryAsync<dynamic>(commandHorarios);
                var horariosPlanos = new List<ProfesorHorarioInfo>();

                // TRADUCTOR DE LECTURA: De 15 min (DB) a 1 Hora (Angular)
                foreach (var fila in pivotHorarios)
                {
                    int idHoraLocal = Convert.ToInt32(fila.idhora); // Viene del 1 al 56
                    int uiHora = (int)Math.Ceiling(idHoraLocal / 4.0); // Lo convierte del 1 al 14 para Angular

                    // Función local para evitar duplicar el check de 1 hora en Angular
                    void AgregarCheck(int idDia)
                    {
                        if (!horariosPlanos.Any(x => x.IdHora == uiHora && x.IdDia == idDia))
                        {
                            horariosPlanos.Add(new ProfesorHorarioInfo { IdHora = uiHora, IdDia = idDia });
                        }
                    }

                    if (fila.stLunes?.ToString().Trim().ToUpper() == "D") AgregarCheck(1);
                    if (fila.stMartes?.ToString().Trim().ToUpper() == "D") AgregarCheck(2);
                    if (fila.stMiercoles?.ToString().Trim().ToUpper() == "D") AgregarCheck(3);
                    if (fila.stJueves?.ToString().Trim().ToUpper() == "D") AgregarCheck(4);
                    if (fila.stViernes?.ToString().Trim().ToUpper() == "D") AgregarCheck(5);
                    if (fila.stSabado?.ToString().Trim().ToUpper() == "D") AgregarCheck(6);
                    if (fila.stDomingo?.ToString().Trim().ToUpper() == "D") AgregarCheck(7);
                }

                info.Disponibilidad.Horarios = horariosPlanos;
            }
        }

        return info;
    }

    public async Task<(int IdActor, string Codigo)> CreateAsync(
        Profesor profesorData, string usuario, string contrasena, int idTenor, bool genero, int idCivil,
        string emailOpcional, string emailAdicional, DateTime fechaNacimiento, int? idPaisNacimiento,
        string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular,
        string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia,
        int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad,
        string codigoPostal, int? idDocumento, string ruc, string estado, int? idPeriodo,
        int usuarioCreacionId, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@usuario", usuario);
        parameters.Add("@contrasena", contrasena);
        parameters.Add("@paterno", profesorData.ApellidoPaterno);
        parameters.Add("@materno", profesorData.ApellidoMaterno);
        parameters.Add("@nombres", profesorData.Nombres);
        parameters.Add("@idtenor", idTenor);
        parameters.Add("@genero", genero);
        parameters.Add("@idcivil", idCivil);
        parameters.Add("@email", profesorData.Email);
        parameters.Add("@emailopcional", emailOpcional);
        parameters.Add("@emailadicional", emailAdicional);
        parameters.Add("@fechanacimiento", fechaNacimiento);
        parameters.Add("@idpaisnacimiento", idPaisNacimiento ?? -1);
        parameters.Add("@idubigeonacimiento", string.IsNullOrEmpty(idUbigeoNacimiento) ? "-1" : idUbigeoNacimiento);
        parameters.Add("@idubigeonacimientootro", idUbigeoNacimientoOtro ?? "");
        parameters.Add("@telefono", telefono);
        parameters.Add("@celular", celular);
        parameters.Add("@telefonoreferencial", telefonoReferencial);
        parameters.Add("@direccion", direccion);
        parameters.Add("@urbanizacion", urbanizacion);
        parameters.Add("@direccionreferencia", direccionReferencia);
        parameters.Add("@idpais", idPais ?? -1);
        parameters.Add("@idubigeo", string.IsNullOrEmpty(idUbigeo) ? "-1" : idUbigeo);
        parameters.Add("@idubigeoresidenciaotro", idUbigeoResidenciaOtro ?? "");
        parameters.Add("@idnacionalidad", idNacionalidad ?? -1);
        parameters.Add("@codigopostal", codigoPostal);
        parameters.Add("@iddocumento", idDocumento ?? -1);
        parameters.Add("@numerodocumento", profesorData.Documento);
        parameters.Add("@ruc", ruc);
        parameters.Add("@estado", estado);
        parameters.Add("@idperiodo", idPeriodo ?? -1);
        parameters.Add("@usuariocreacion", usuarioCreacionId);

        parameters.Add("@retval", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

        var command = new CommandDefinition(
            commandText: "up_ins_actor_xprofesor",
            parameters: parameters,
            transaction: transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        await transaction.Connection.ExecuteAsync(command);

        string output = parameters.Get<string>("@retval");
        if (!string.IsNullOrEmpty(output) && output.Contains('|'))
        {
            var parts = output.Split('|');
            return (int.Parse(parts[0]), parts[1]);
        }

        throw new Exception("Error al crear el profesor: El SP no devolvió el formato esperado.");
    }

    public async Task<bool> UpdateAsync(
        int idActor, Profesor profesorData, string usuario, int idTenor, bool sexo, int idCivil,
        string emailOpcional, string emailAdicional, DateTime fechaNacimiento, int? idPaisNacimiento,
        string idUbigeoNacimiento, string idUbigeoNacimientoOtro, string telefono, string celular,
        string telefonoReferencial, string direccion, string urbanizacion, string direccionReferencia,
        int? idPais, string idUbigeo, string idUbigeoResidenciaOtro, int? idNacionalidad,
        string codigoPostal, int? idDocumento, string ruc, string estado, int usuarioModificacionId,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@idactor", idActor);
        parameters.Add("@usuario", usuario);
        parameters.Add("@paterno", profesorData.ApellidoPaterno);
        parameters.Add("@materno", profesorData.ApellidoMaterno);
        parameters.Add("@nombres", profesorData.Nombres);
        parameters.Add("@idtenor", idTenor);
        parameters.Add("@sexo", sexo);
        parameters.Add("@idcivil", idCivil);
        parameters.Add("@email", profesorData.Email);
        parameters.Add("@emailopcional", emailOpcional);
        parameters.Add("@emailadicional", emailAdicional);
        parameters.Add("@fechanacimiento", fechaNacimiento);
        parameters.Add("@idpaisnacimiento", idPaisNacimiento ?? -1);
        parameters.Add("@idubigeonacimiento", string.IsNullOrEmpty(idUbigeoNacimiento) ? "-1" : idUbigeoNacimiento);
        parameters.Add("@idubigeonacimientootro", idUbigeoNacimientoOtro ?? "");
        parameters.Add("@telefono", telefono);
        parameters.Add("@celular", celular);
        parameters.Add("@telefonoreferencial", telefonoReferencial);
        parameters.Add("@direccion", direccion);
        parameters.Add("@urbanizacion", urbanizacion);
        parameters.Add("@direccionreferencia", direccionReferencia);
        parameters.Add("@idpais", idPais ?? -1);
        parameters.Add("@idubigeo", string.IsNullOrEmpty(idUbigeo) ? "-1" : idUbigeo);
        parameters.Add("@idubigeoresidenciaotro", idUbigeoResidenciaOtro ?? "");
        parameters.Add("@idnacionalidad", idNacionalidad ?? -1);
        parameters.Add("@codigopostal", codigoPostal);
        parameters.Add("@iddocumento", idDocumento ?? -1);
        parameters.Add("@numerodocumento", profesorData.Documento);
        parameters.Add("@ruc", ruc);
        parameters.Add("@estado", estado);
        parameters.Add("@usuariomodificacion", usuarioModificacionId);

        using var connection = context.CreateConnection();

        var command = new CommandDefinition(
            commandText: "up_upd_actor_xprofesor",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var affectedRows = await connection.ExecuteAsync(command);

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@idactor", id);
        parameters.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = context.CreateConnection();

        var commandDefinition = new CommandDefinition(
            commandText: "up_del_actor_xidprofesor",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(commandDefinition);

        int resultValue = parameters.Get<int>("@retval");

        if (resultValue == 0) return true;

        throw new InvalidOperationException(resultValue switch
        {
            1 => "No se puede eliminar: El profesor tiene cursos asignados.",
            2 => "No se puede eliminar: El profesor tiene bloqueos de horario registrados.",
            _ => "No se pudo eliminar el profesor debido a restricciones de integridad en la base de datos."
        });
    }

    public async Task AsignarCursoAsync(
        int idActor, int idCurso, double tarifa, int usuarioCreacionId,
        IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var connection = transaction.Connection;

        string checkQuery = "SELECT COUNT(1) FROM profesorcurso WHERE idactor = @IdActor AND idcurso = @IdCurso";

        var checkCommand = new CommandDefinition(
            commandText: checkQuery,
            parameters: new { IdActor = idActor, IdCurso = idCurso },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        int existe = await connection.ExecuteScalarAsync<int>(checkCommand);

        if (existe > 0)
        {
            var updParams = new DynamicParameters();
            updParams.Add("@idactor", idActor);
            updParams.Add("@idcurso", idCurso);
            updParams.Add("@tarifa", tarifa);
            updParams.Add("@usuariomodificacion", usuarioCreacionId);

            var updCommand = new CommandDefinition(
                commandText: "up_upd_profesorcurso",
                parameters: updParams,
                transaction: transaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await connection.ExecuteAsync(updCommand);
        }
        else
        {
            var insParams = new DynamicParameters();
            insParams.Add("@idactor", idActor);
            insParams.Add("@idcurso", idCurso);
            insParams.Add("@tarifa", tarifa);
            insParams.Add("@usuariocreacion", usuarioCreacionId);

            var insCommand = new CommandDefinition(
                commandText: "up_ins_profesorcurso",
                parameters: insParams,
                transaction: transaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await connection.ExecuteAsync(insCommand);
        }
    }

    public async Task<int> InsertarDisponibilidadAsync(
        int idActor, DateTime fechaInicio, DateTime fechaFin, int usuarioCreacionId,
        IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@idactor", idActor);
        parameters.Add("@fechainicio", fechaInicio);
        parameters.Add("@fechafin", fechaFin);
        parameters.Add("@usuariocreacion", usuarioCreacionId);
        parameters.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText: "up_ins_profesordisponibilidad",
            parameters: parameters,
            transaction: transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        await transaction.Connection.ExecuteAsync(command);

        return parameters.Get<int>("@retval");
    }

    public async Task InsertarHorarioAsync(
        int idDisponibilidad, int idHora, int idDia,
        IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@iddisponibilidad", idDisponibilidad);
        parameters.Add("@idsemana", 1);
        parameters.Add("@idhora", idHora);
        parameters.Add("@iddia", idDia);

        var command = new CommandDefinition(
            commandText: "up_ins_profesorhorario",
            parameters: parameters,
            transaction: transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        await transaction.Connection.ExecuteAsync(command);
    }

    public async Task ReemplazarCursosYHorariosAsync(
        int idActor,
        IEnumerable<ProfesorCursoInfo> cursos,
        ProfesorDisponibilidadInfo disponibilidad,
        int usuarioModificacionId,
        CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. REEMPLAZO DE CURSOS
            await connection.ExecuteAsync("DELETE FROM profesorcurso WHERE idactor = @IdActor", new { IdActor = idActor }, transaction);

            if (cursos != null)
            {
                foreach (var c in cursos)
                {
                    await AsignarCursoAsync(idActor, c.IdCurso, (double)c.Tarifa, usuarioModificacionId, transaction, cancellationToken);
                }
            }

            // 2. ACTUALIZACIÓN / REEMPLAZO DE DISPONIBILIDAD HORARIA
            if (disponibilidad != null && disponibilidad.FechaInicio.HasValue && disponibilidad.FechaFin.HasValue)
            {
                string queryIdDispo = "SELECT iddisponibilidad FROM profesordisponibilidad WHERE idactor = @IdActor";
                int? idDispo = await connection.QueryFirstOrDefaultAsync<int?>(queryIdDispo, new { IdActor = idActor }, transaction);

                if (idDispo.HasValue)
                {
                    string updateDispo = "UPDATE profesordisponibilidad SET fechainicio = @FI, fechafin = @FF WHERE iddisponibilidad = @IdDispo";
                    await connection.ExecuteAsync(updateDispo, new
                    {
                        FI = disponibilidad.FechaInicio.Value,
                        FF = disponibilidad.FechaFin.Value,
                        IdDispo = idDispo.Value
                    }, transaction);

                    // Borrar el detalle anterior
                    await connection.ExecuteAsync("DELETE FROM profesorhorario WHERE iddisponibilidad = @IdDispo", new { IdDispo = idDispo.Value }, transaction);
                }
                else
                {
                    idDispo = await InsertarDisponibilidadAsync(idActor, disponibilidad.FechaInicio.Value, disponibilidad.FechaFin.Value, usuarioModificacionId, transaction, cancellationToken);
                }

                if (disponibilidad.Horarios != null && idDispo.HasValue)
                {
                    // PRIMERO: Desactivamos todos los días (Los marcamos con 'X')
                    await connection.ExecuteAsync("UPDATE profesorsemanadia SET diaestado = 'X' WHERE iddisponibilidad = @IdDispo", new { IdDispo = idDispo.Value }, transaction);

                    // TRADUCTOR DE ESCRITURA: De 1 Hora (Angular) a 15 Min (DB)
                    foreach (var h in disponibilidad.Horarios)
                    {
                        // Si Angular manda la fila 1, la DB inserta 1,2,3 y 4.
                        // Si Angular manda la fila 2, la DB inserta 5,6,7 y 8.
                        int dbHoraStart = ((h.IdHora - 1) * 4) + 1;

                        for (int i = 0; i < 4; i++)
                        {
                            int dbHoraActual = dbHoraStart + i;
                            await InsertarHorarioAsync(idDispo.Value, dbHoraActual, h.IdDia, transaction, cancellationToken);
                        }

                        // SEGUNDO: Como este día sí tiene horas, lo activamos poniéndole 'A'
                        await connection.ExecuteAsync(
                            "UPDATE profesorsemanadia SET diaestado = 'A' WHERE iddisponibilidad = @IdDispo AND iddia = @IdDia",
                            new { IdDispo = idDispo.Value, IdDia = h.IdDia }, transaction);
                    }
                }
            }

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}