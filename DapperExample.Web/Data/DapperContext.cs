using System.Data;
using DapperExample.Web.Contracts;
using Npgsql;

namespace DapperExample.Web.Data;

public class DapperContext : IDapperContext
{
    private readonly string _connectionString;

    public DapperContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}