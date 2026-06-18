using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Text;
using Academic.Application.Interfaces;
using Academic.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Academic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ========================================================
        // 1. CONFIGURACIÓN DE REDIS
        // ========================================================
        var redisSection = configuration.GetSection("Redis");
        var connectionString = redisSection["RedisConnection"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "La configuración de Redis no se encuentra en el appsettings.json.");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            var configOptions = ConfigurationOptions.Parse(connectionString);
            configOptions.AbortOnConnectFail = bool.Parse(redisSection["AbortOnConnectFail"] ?? "false");
            configOptions.ConnectRetry = int.Parse(redisSection["ConnectRetry"] ?? "3");
            configOptions.ConnectTimeout = int.Parse(redisSection["ConnectTimeout"] ?? "15000");
            configOptions.SyncTimeout = int.Parse(redisSection["SyncTimeout"] ?? "5000");
            configOptions.KeepAlive = int.Parse(redisSection["KeepAlive"] ?? "180");

            options.ConfigurationOptions = configOptions;
            options.InstanceName = redisSection["InstanceName"];
        });

        // ========================================================
        // 2. CONFIGURACIÓN DE SEGURIDAD (JWT)
        // ========================================================
        services.AddScoped<IJwtProvider, JwtProvider>();

        var secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new ArgumentNullException("JwtSettings:SecretKey no encontrado. Asegúrate de ejecutar: dotnet user-secrets set \"JwtSettings:SecretKey\" \"TU_CLAVE_DE_32_CARACTERES_MINIMO\"");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // En .NET 10 esto fuerza el uso del nuevo JsonWebTokenHandler optimizado
                options.UseSecurityTokenValidators = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

        services.AddAuthorization();

        return services;
    }
}