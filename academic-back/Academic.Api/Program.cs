using Academic.Api.ExceptionHandlers;
using Academic.Application;
using Academic.Infrastructure;
using Academic.Persistence;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ========================================================
// 1. REGISTRO DE SERVICIOS (DEPENDENCY INJECTION)
// ========================================================
builder.Services.AddApplication();

// AddInfrastructure ahora contiene tanto la configuración de Redis como la de JWT
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddPersistence();
builder.Services.AddControllers();

// Generación de documentación OpenAPI Nativa
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configuración de Observabilidad por defecto (OpenTelemetry)
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200",
                    "https://7p7s35xz-7241.brs.devtunnels.ms",
                    "http://localhost:8081"
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// ========================================================
// 2. CONFIGURACIÓN DEL PIPELINE HTTP (MIDDLEWARES)
// ========================================================
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.Mars;
        options.Title = "Academic API v1";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularFrontend");

// ---> ORDEN CRÍTICO DE SEGURIDAD <---
// Autenticación: Valida quién es el usuario mediante el token JWT
app.UseAuthentication();
// Autorización: Valida si el usuario tiene permiso para acceder al endpoint
app.UseAuthorization();

app.MapControllers();

app.Run();