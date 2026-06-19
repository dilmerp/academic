using System;
using System.Data;
using Academic.Domain.Interfaces;
using Npgsql; 
using Microsoft.Extensions.Configuration;

namespace Academic.Persistence.Data;

public class DapperContext(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new ArgumentNullException("DefaultConnection string is missing");

    // Cambiamos SqlConnection por NpgsqlConnection
    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}