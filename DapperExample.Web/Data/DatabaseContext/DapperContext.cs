using System.Data;
using Dapper;
using DapperExample.Web.Data.Schemas;
using DapperExample.Web.Models;
using FluentMigrator.Runner;
using Npgsql;

namespace DapperExample.Web.Data.DatabaseContext;

public class DapperContext
{
    private readonly string _applicationDbConnection;
    private readonly string _masterDbConnection;
    private readonly string _applicationDbName;
    private Database? _database;

    public DapperContext(string connectionString)
    {
        _applicationDbConnection = connectionString;
        _masterDbConnection = GetMasterDbConnection(_applicationDbConnection);
        _applicationDbName = GetDatabaseName(_applicationDbConnection);
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_applicationDbConnection);

    public IDbConnection CreateConnectionMaster()
        => new NpgsqlConnection(_masterDbConnection);

    public string ApplicationDbName => _applicationDbName;

    public Database Database
    {
        get { return _database ??= new Database(this); }
    }

    public int AddBooks(IEnumerable<Book> items)
    {
        const string query =
            $@"INSERT INTO ""{BookSchema.Table}"" (""{BookSchema.Columns.Id}"", ""{BookSchema.Columns.Title}"", ""{BookSchema.Columns.PublishedOn}"") " +
            $@"VALUES (@{BookSchema.Columns.Id}, @{BookSchema.Columns.Title}, @{BookSchema.Columns.PublishedOn})";

        using var connection = CreateConnection();
        var parametersList = items.Select(d => new DynamicParameters(d));
        return connection.Execute(query, parametersList);
    }

    public int AddReviews(IEnumerable<Review> items)
    {
        const string query =
            $@"INSERT INTO ""{ReviewSchema.Table}"" (" +
            $@"""{ReviewSchema.Columns.Id}"", " +
            $@"""{ReviewSchema.Columns.Comment}"", " +
            $@"""{ReviewSchema.Columns.Rating}"", " +
            $@"""{ReviewSchema.Columns.BookId}"") " +
            $@"VALUES (" +
            $@"@{ReviewSchema.Columns.Id}, " +
            $@"@{ReviewSchema.Columns.Comment}, " +
            $@"@{ReviewSchema.Columns.Rating}, " +
            $@"@{ReviewSchema.Columns.BookId})";

        using var connection = CreateConnection();
        var parametersList = items.Select(d => new DynamicParameters(d));
        return connection.Execute(query, parametersList);
    }

    private string GetDatabaseName(string connectionString)
    {
        return connectionString
            .Split(';')
            .SingleOrDefault(x => x.ToLower().Contains("database="))
            ?.Split('=')
            .ElementAt(1) ?? string.Empty;
    }

    private string GetMasterDbConnection(string connectionString)
    {
        var dbConnection = connectionString.Split(';').Where(x => !x.ToLower().Contains("database="));
        return string.Join(';', dbConnection);
    }
}