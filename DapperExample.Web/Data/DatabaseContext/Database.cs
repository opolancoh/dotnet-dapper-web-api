using System.Reflection;
using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;

namespace DapperExample.Web.Data.DatabaseContext;

public sealed class Database
{
    private readonly DapperContext _context;
    private readonly IMigrationRunner _migrationRunner;
    
    public Database(DapperContext context)
    {
        _context = context;
        // _migrationRunner = migrationRunner;
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
        connection.Execute($"DROP DATABASE {_context.ApplicationDbName}");
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

    public static bool RunMigrations(string databaseType, string connectionString, Assembly migrationsAssembly)
    {


        return true;
    }

    private string DbExistsQuery => "SELECT 1 FROM pg_database WHERE datname = @name";
}