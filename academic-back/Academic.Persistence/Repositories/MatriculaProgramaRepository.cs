using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using Academic.Persistence.Data;
using Dapper;

namespace Academic.Persistence.Repositories;

public class MatriculaProgramaRepository(DapperContext context) : IMatriculaProgramaRepository
{
    public async Task<string> RegistrarMatriculaProgramaAsync(
        int idAlumno, int idRegistro, string alumnoCodigo, int idContacto, string paterno,
        string materno, string nombres, int idTenor, bool sexo, int idDocumento,
        string numeroDocumento, int idCivil, string direccion, string urbanizacion,
        int idPais, string idUbigeo, string telefono, string celular, string fax,
        string email, int idTipo, int idSubTipo, int idProducto, int idPromocion,
        int idGrupo, int idCursoBase, int idSeccionBase, int idMedio, int idBeca,
        int idPlan, int idMoneda, int idPeriodo, int idFacultad, int idMora, int usuarioId,
        IEnumerable<CursoMatriculaInfo> cursos,
        IEnumerable<CuotaMatriculaInfo> cuotas,
        CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            int currentIdAlumno = idAlumno;
            string currentCodigoAlumno = alumnoCodigo;

            if (currentIdAlumno == -1)
            {
                var actorParams = new DynamicParameters();
                actorParams.Add("@contrasena", Guid.NewGuid().ToString()[..8]);
                actorParams.Add("@paterno", paterno);
                actorParams.Add("@materno", materno);
                actorParams.Add("@nombres", nombres);
                actorParams.Add("@idtenor", idTenor == -1 ? null : idTenor);
                actorParams.Add("@sexo", sexo);
                actorParams.Add("@iddocumento", idDocumento == -1 ? null : idDocumento);
                actorParams.Add("@numerodocumento", numeroDocumento);
                actorParams.Add("@idcivil", idCivil == -1 ? null : idCivil);
                actorParams.Add("@direccion", direccion);
                actorParams.Add("@urbanizacion", urbanizacion);
                actorParams.Add("@idpais", idPais == -1 ? null : idPais);
                actorParams.Add("@idubigeo", idUbigeo == "-1" ? null : idUbigeo);
                actorParams.Add("@telefono", telefono);
                actorParams.Add("@celular", celular);
                actorParams.Add("@fax", fax);
                actorParams.Add("@email", email);
                actorParams.Add("@idperiodo", idPeriodo);
                actorParams.Add("@usuariocreacion", usuarioId);
                actorParams.Add("@retval", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

                var cmdActor = new CommandDefinition("up_ins_actor_xmatricula", actorParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(cmdActor);

                string retval = actorParams.Get<string>("@retval");
                if (!string.IsNullOrEmpty(retval) && retval.Contains('|'))
                {
                    var split = retval.Split('|');
                    currentIdAlumno = int.Parse(split[0]);
                    currentCodigoAlumno = split[1];
                }
            }
            else
            {
                var actorUpdParams = new DynamicParameters();
                actorUpdParams.Add("@idactor", currentIdAlumno);
                actorUpdParams.Add("@paterno", paterno);
                actorUpdParams.Add("@materno", materno);
                actorUpdParams.Add("@nombres", nombres);
                actorUpdParams.Add("@idtenor", idTenor == -1 ? null : idTenor);
                actorUpdParams.Add("@sexo", sexo);
                actorUpdParams.Add("@iddocumento", idDocumento == -1 ? null : idDocumento);
                actorUpdParams.Add("@numerodocumento", numeroDocumento);
                actorUpdParams.Add("@idcivil", idCivil == -1 ? null : idCivil);
                actorUpdParams.Add("@direccion", direccion);
                actorUpdParams.Add("@urbanizacion", urbanizacion);
                actorUpdParams.Add("@idpais", idPais == -1 ? null : idPais);
                actorUpdParams.Add("@idubigeo", idUbigeo == "-1" ? null : idUbigeo);
                actorUpdParams.Add("@telefono", telefono);
                actorUpdParams.Add("@celular", celular);
                actorUpdParams.Add("@fax", fax);
                actorUpdParams.Add("@email", email);
                actorUpdParams.Add("@usuariomodificacion", usuarioId);

                var cmdActorUpd = new CommandDefinition("up_upd_actor_xmatricula", actorUpdParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(cmdActorUpd);
            }

            int currentIdRegistro = idRegistro;
            if (currentIdRegistro == -1)
            {
                var registroParams = new DynamicParameters();
                registroParams.Add("@idalumno", currentIdAlumno);
                registroParams.Add("@idtipo", idTipo);
                registroParams.Add("@idsubtipo", idSubTipo);
                registroParams.Add("@idproducto", idProducto);
                registroParams.Add("@idpromocion", idPromocion);
                registroParams.Add("@idmedio", idMedio);
                registroParams.Add("@estado", "A");
                registroParams.Add("@usuariocreacion", usuarioId);
                registroParams.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var cmdRegistro = new CommandDefinition("up_ins_alumnoregistro", registroParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(cmdRegistro);
                currentIdRegistro = registroParams.Get<int>("@retval");
            }

            var matriculaParams = new DynamicParameters();
            matriculaParams.Add("@idalumno", currentIdAlumno);
            matriculaParams.Add("@idregistro", currentIdRegistro);
            matriculaParams.Add("@idproducto", idProducto);
            matriculaParams.Add("@idpromocion", idPromocion);
            matriculaParams.Add("@idgrupo", idGrupo);
            matriculaParams.Add("@idcurso", idCursoBase == -1 ? null : idCursoBase);
            matriculaParams.Add("@idseccion", idSeccionBase == -1 ? null : idSeccionBase);
            matriculaParams.Add("@matriculafecha", DateTime.Now);
            matriculaParams.Add("@matriculausuario", usuarioId);
            matriculaParams.Add("@idmedio", idMedio);
            matriculaParams.Add("@idbeca", idBeca == -1 ? null : idBeca);
            matriculaParams.Add("@esmatricula", false);
            matriculaParams.Add("@estado", "A");
            matriculaParams.Add("@usuariocreacion", usuarioId);
            matriculaParams.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var cmdMatricula = new CommandDefinition("up_ins_alumnomatricula", matriculaParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(cmdMatricula);
            int currentIdMatricula = matriculaParams.Get<int>("@retval");

            foreach (var curso in cursos)
            {
                var cursoParams = new DynamicParameters();
                cursoParams.Add("@idalumno", currentIdAlumno);
                cursoParams.Add("@idregistro", currentIdRegistro);
                cursoParams.Add("@idmatricula", currentIdMatricula);
                cursoParams.Add("@idproducto", idProducto);
                cursoParams.Add("@idpromocion", idPromocion);
                cursoParams.Add("@idgrupo", idGrupo);
                cursoParams.Add("@idcurso", curso.IdCurso);
                cursoParams.Add("@idseccion", curso.IdSeccion == -1 ? null : curso.IdSeccion);
                cursoParams.Add("@esmatricula", false);
                cursoParams.Add("@usuariocreacion", usuarioId);
                cursoParams.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var cmdCurso = new CommandDefinition("up_ins_alumnocurso", cursoParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(cmdCurso);
                int currentIdAlumnoCurso = cursoParams.Get<int>("@retval");

                var curriculaParams = new DynamicParameters();
                curriculaParams.Add("@idalumno", currentIdAlumno);
                curriculaParams.Add("@idregistro", currentIdRegistro);
                curriculaParams.Add("@idmodulo", curso.IdModulo);
                curriculaParams.Add("@idcurso", curso.IdCurso);
                curriculaParams.Add("@usuariocreacion", usuarioId);
                curriculaParams.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var cmdCurricula = new CommandDefinition("up_ins_alumnocurricula", curriculaParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(cmdCurricula);
                int currentIdCurricula = curriculaParams.Get<int>("@retval");

                var linkParams = new DynamicParameters();
                linkParams.Add("@idalumno", currentIdAlumno);
                linkParams.Add("@idregistro", currentIdRegistro);
                linkParams.Add("@idcurricula", currentIdCurricula);
                linkParams.Add("@idalumnocurso", currentIdAlumnoCurso);

                var cmdLink = new CommandDefinition("up_ins_alumnocurriculacurso", linkParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(cmdLink);
            }

            var movParams = new DynamicParameters();
            movParams.Add("@idfacultad", idFacultad);
            movParams.Add("@idalumno", currentIdAlumno);
            movParams.Add("@idregistro", currentIdRegistro);
            movParams.Add("@idmatricula", currentIdMatricula);
            movParams.Add("@idplan", idPlan);
            movParams.Add("@esfacturado", false);
            movParams.Add("@usuariocreacion", usuarioId);
            movParams.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var cmdMov = new CommandDefinition("up_ins_movimiento", movParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(cmdMov);
            int currentIdMovimiento = movParams.Get<int>("@retval");

            foreach (var cuota in cuotas)
            {
                var cuotaParams = new DynamicParameters();
                cuotaParams.Add("@idfacultad", idFacultad);
                cuotaParams.Add("@idmovimiento", currentIdMovimiento);
                cuotaParams.Add("@idalumno", currentIdAlumno);
                cuotaParams.Add("@idtipomovimiento", 1);
                cuotaParams.Add("@cuota", cuota.Cuota);
                cuotaParams.Add("@cuotadescripcion", cuota.PrecioDescripcion);
                cuotaParams.Add("@cuotas", cuota.Cuotas);
                cuotaParams.Add("@totalcuotas", cuota.TotalCuotas);
                cuotaParams.Add("@idconcepto", cuota.IdConcepto);
                cuotaParams.Add("@idprecio", cuota.IdPrecio);
                cuotaParams.Add("@idmoneda", idMoneda);
                cuotaParams.Add("@precio", cuota.Precio);
                cuotaParams.Add("@idmora", idMora);
                cuotaParams.Add("@vencimiento", cuota.Vencimiento);
                cuotaParams.Add("@escontado", cuota.EsContado);
                cuotaParams.Add("@esinicial", cuota.EsInicial);
                cuotaParams.Add("@escuota", cuota.EsCuota);
                cuotaParams.Add("@esobligatorio", cuota.EsObligatorio);
                cuotaParams.Add("@estado", "P");
                cuotaParams.Add("@usuariocreacion", usuarioId);

                var cmdCuota = new CommandDefinition("up_ins_programacioncuota", cuotaParams, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(cmdCuota);
            }

            transaction.Commit();
            return $"{currentIdAlumno}|{currentCodigoAlumno}";
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    // ========================================================
    // MÉTODO AGREGADO PARA EL LISTADO DE MATRÍCULAS (GET)
    // ========================================================
    public async Task<IEnumerable<dynamic>> GetMatriculasProgramaAsync(CancellationToken cancellationToken)
    {
        // Reemplaza "usp_ListarMatriculasPrograma" por el nombre real de tu procedimiento almacenado o vista
        var query = "up_sel_alumnomatriculaprograma";

        using var connection = context.CreateConnection();

        var commandDefinition = new CommandDefinition(
            query,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        return await connection.QueryAsync<dynamic>(commandDefinition);
    }
}