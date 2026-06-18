using Academic.Domain.Entities;
using Academic.Domain.Interfaces;
using Academic.Persistence.Data;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academic.Persistence.Repositories;

public class MaestroRepository(DapperContext context) : IMaestroRepository
{
    public async Task<IEnumerable<MaestroItem>> GetTenoresAsync(CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        // Se cambia a SELECT * FROM function()
        var command = new CommandDefinition("SELECT * FROM up_sel_tenor_xactivo()", cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new MaestroItem
        {
            Id = x.idtenor,
            Codigo = x.tenorcodigo,
            Nombre = x.tenornombre
        });
    }

    //public async Task<IEnumerable<MaestroItem>> GetEstadosCivilesAsync(CancellationToken cancellationToken)
    //{
    //    using var connection = context.CreateConnection();
    //    var command = new CommandDefinition("SELECT * FROM up_sel_estadocivil_xactivo()", cancellationToken: cancellationToken);
    //    var result = await connection.QueryAsync<dynamic>(command);

    //    return result.Select(x => new MaestroItem
    //    {
    //        Id = x.idestadocivil,
    //        Codigo = x.estadocivilcodigo,
    //        Nombre = x.estadocivilnombre
    //    });
    //}

    public async Task<IEnumerable<MaestroItem>> GetEstadosCivilesAsync(CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var command = new CommandDefinition("SELECT * FROM up_sel_estadocivil_xactivo()", cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new MaestroItem
        {
            Id = (int)x.id,
            Codigo = x.codigo?.ToString() ?? "",
            Nombre = x.nombre?.ToString() ?? ""
        });
    }

    public async Task<IEnumerable<MaestroItem>> GetTiposDocumentoAsync(CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var command = new CommandDefinition("SELECT * FROM up_sel_documentotipo_xactivo()", cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new MaestroItem
        {
            Id = x.idtipodocumento,
            Codigo = x.tipodocumentocodigo,
            Nombre = x.tipodocumentonombre
        });
    }

    public async Task<IEnumerable<MaestroItem>> GetCursosAsync(CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var command = new CommandDefinition("SELECT * FROM up_sel_curso()", cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new MaestroItem
        {
            Id = x.idcurso,
            Codigo = x.cursocodigo,
            Nombre = x.cursonombre
        });
    }

    public async Task<IEnumerable<MaestroItem>> GetPromocionesAsync(CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var command = new CommandDefinition("SELECT * FROM up_sel_promociones_maestro()", cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new MaestroItem
        {
            Id = x.idpromocion,
            Codigo = x.promocioncodigo,
            Nombre = x.promocionnombre
        });
    }

    public async Task<IEnumerable<GrupoPromocionItem>> GetGruposByPromocionAsync(int idPromocion, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var parameters = new { idpromocion = idPromocion };

        // Se inyectan los parámetros en la sintaxis de PostgreSQL
        var command = new CommandDefinition(
            "SELECT * FROM up_sel_grupo_xidpromocion(@idpromocion)",
            parameters,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new GrupoPromocionItem
        {
            Id = x.idgrupo,
            Codigo = x.grupocodigo,
            Nombre = x.gruponombre,
            IdTipo = x.idtipo != null ? (int)x.idtipo : 0,
            IdSubTipo = x.idsubtipo != null ? (int)x.idsubtipo : 0,
            IdProducto = x.idproducto != null ? (int)x.idproducto : 0,
            IdModulo = x.idmodulo != null ? (int)x.idmodulo : 0
        });
    }

    public async Task<IEnumerable<MaestroItem>> GetCursosByPromocionAsync(int idPromocion, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();
        var parameters = new { idpromocion = idPromocion };

        var command = new CommandDefinition(
            "SELECT * FROM up_sel_promocioncursoseccion_xidpromocion(@idpromocion)",
            parameters,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<dynamic>(command);

        return result
            .GroupBy(x => (int)x.idcurso)
            .Select(group =>
            {
                var first = group.First();
                return new MaestroItem
                {
                    Id = first.idcurso,
                    Codigo = first.seccioncodigo ?? string.Empty,
                    Nombre = first.cursonombre
                };
            });
    }

    public async Task<IEnumerable<PlanPagoItem>> GetPlanesPagoAsync(int idPromocion, int idGrupo, int idSeccion, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        var parameters = new
        {
            idpromocion = idPromocion,
            idgrupo = idGrupo,
            idseccion = idSeccion
        };

        var command = new CommandDefinition(
            "SELECT * FROM up_sel_planpago_xidpromocion_xidgrupo_xidseccion(@idpromocion, @idgrupo, @idseccion)",
            parameters,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new PlanPagoItem
        {
            IdPlan = x.idplan,
            PlanNombre = x.plannombre,
            IdMoneda = x.idmoneda,
            MonedaNombre = x.monedanombre
        });
    }

    public async Task<IEnumerable<BecaItem>> GetBecasAsync(int idActor, int idPromocion, CancellationToken cancellationToken)
    {
        using var connection = context.CreateConnection();

        var parameters = new
        {
            idactor = idActor,
            idpromocion = idPromocion
        };

        var command = new CommandDefinition(
            "SELECT * FROM up_sel_beca_xidactor_xidpromocion(@idactor, @idpromocion)",
            parameters,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<dynamic>(command);

        return result.Select(x => new BecaItem
        {
            IdBeca = x.idbeca,
            BeneficioNombre = x.beneficionombre,
            Descuento = x.descuento != null ? (decimal)x.descuento : 0,
            Descripcion = x.descripcion ?? string.Empty
        });
    }
}