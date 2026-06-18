using Academic.Domain.Interfaces;
using Academic.Persistence.Data;
using Academic.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Academic.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        // Registro del Contexto de Dapper (Asegúrate de ajustar el ciclo de vida según lo tenías)
        services.AddSingleton<DapperContext>();
        // Registro de los Repositorios
        services.AddScoped<IAlumnoRepository, AlumnoRepository>();
        services.AddScoped<IProfesorRepository, ProfesorRepository>();
        services.AddSingleton<IDbConnectionFactory, DapperContext>();
        services.AddScoped<IMaestroRepository, MaestroRepository>();
        services.AddScoped<IMatriculaCarreraRepository, MatriculaCarreraRepository>();
        services.AddScoped<IMatriculaProgramaRepository , MatriculaProgramaRepository>();
        services.AddScoped<IValidacionMatriculaRepository, ValidacionMatriculaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        return services;
    }
}