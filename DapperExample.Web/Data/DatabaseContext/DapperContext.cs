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

    public void AddBooks(IEnumerable<Book> items)
    {
        const string query =
            $@"INSERT INTO ""{BookSchema.Table}"" (""{BookSchema.Columns.Id}"", ""{BookSchema.Columns.Title}"", ""{BookSchema.Columns.PublishedOn}"") " +
            $@"VALUES (@{BookSchema.Columns.Id}, @{BookSchema.Columns.Title}, @{BookSchema.Columns.PublishedOn})";


        var connection = CreateConnection();
        try
        {
            connection.Open();
            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                parameters.Add(BookSchema.Columns.Id, item.Id, DbType.Guid);
                parameters.Add(BookSchema.Columns.Title, item.Title, DbType.String);
                parameters.Add(BookSchema.Columns.PublishedOn, item.PublishedOn, DbType.DateTime);

                var result = connection.Execute(query, parameters);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception executing AddBooks(): {e}");
        }
        finally
        {
            connection.Close();
        }
    }

    public void AddReviews(IEnumerable<Review> items)
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

        var connection = CreateConnection();
        try
        {
            connection.Open();
            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                parameters.Add(ReviewSchema.Columns.Id, item.Id, DbType.Guid);
                parameters.Add(ReviewSchema.Columns.Comment, item.Comment, DbType.String);
                parameters.Add(ReviewSchema.Columns.Rating, item.Rating, DbType.Int32);
                parameters.Add(ReviewSchema.Columns.BookId, item.BookId, DbType.Guid);

                var result = connection.Execute(query, parameters);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception executing AddBooks(): {e}");
        }
        finally
        {
            connection.Close();
        }
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