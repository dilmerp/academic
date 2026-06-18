using System.Data;

namespace Academic.Domain.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}