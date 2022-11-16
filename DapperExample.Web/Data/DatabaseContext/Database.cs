using Dapper;

namespace DapperExample.Web.Data.DatabaseContext;

public sealed class Database
{
    private readonly DapperContext _context;

    public Database(DapperContext context)
    {
        _context = context;
    }

    // Check if the database does exist
    public bool Exists()
    {
        var parameters = new DynamicParameters();
        parameters.Add("name", _context.ApplicationDbName);

        using var connection = _context.CreateConnectionMaster();
        var result = connection.Query(DbExistsQuery, parameters);

        return result.Any();
    }

    // Ensures that the database for the context does not exist.
    // If it does not exist, no action is taken.
    // If it does exist then the database is deleted.
    public bool EnsureDeleted()
    {
        var dbExists = Exists();

        if (!dbExists) return false;

        using var connection = _context.CreateConnectionMaster();
        var queryList = new string[]
        {
            $"REVOKE CONNECT ON DATABASE {_context.ApplicationDbName} FROM PUBLIC;",
            $"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE datname = '{_context.ApplicationDbName}'",
            $"DROP DATABASE {_context.ApplicationDbName}"
        };

        connection.Execute(String.Join(';', queryList));
        return true;
    }

    // Ensures that the database for the context exists.
    // If it does exists, then no action is taken
    // If if does not exist, then the database is created.
    public bool EnsureCreated()
    {
        var dbExists = Exists();

        if (dbExists) return false;

        using var connection = _context.CreateConnectionMaster();
        connection.Execute($"CREATE DATABASE {_context.ApplicationDbName}");
        return true;
    }

    private string DbExistsQuery => "SELECT 1 FROM pg_database WHERE datname = @name";
}