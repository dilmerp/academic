using System;
using System.Collections.Generic;
using System.Text;
using Academic.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Academic.Infrastructure.Authentication;

public class JwtProvider(IConfiguration configuration) : IJwtProvider
{
    public string Generate(int idUsuario, string nombreCompleto, string rol)
    {
        // Obtenemos la llave secreta desde los user-secrets
        var secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new ArgumentNullException("JwtSettings:SecretKey no está configurado.");

        // Creamos la llave de seguridad para la firma
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Definimos los claims (información contenida en el token)
        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, idUsuario.ToString() },
            { JwtRegisteredClaimNames.Name, nombreCompleto },
            { "rol", rol }
        };

        // Configuramos el descriptor del token
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = configuration["JwtSettings:Issuer"] ?? "AcademicApi",
            Audience = configuration["JwtSettings:Audience"] ?? "AcademicApp",
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "120")),
            SigningCredentials = credentials,
            Claims = claims
        };

        // Generamos el token usando el handler optimizado
        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}