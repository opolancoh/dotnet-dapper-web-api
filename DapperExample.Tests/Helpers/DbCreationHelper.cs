using System.Data;
using Dapper;
using DapperExample.Web.Contracts;
using DapperExample.Web.Data.Schemas;
using DapperExample.Web.Models;

namespace DapperExample.Tests.Helpers;

public class DbCreationHelper
{
    private readonly IDapperContext _context;
    private readonly string _dbName;

    public DbCreationHelper(IDapperContext context, string dbName)
    {
        _context = context;
        _dbName = dbName;
    }

    public void EnsureDeleted()
    {
        const string query = $@"
            REVOKE CONNECT ON DATABASE books_dapper_db_test FROM PUBLIC;
            SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE datname = 'books_dapper_db_test';
            DROP DATABASE IF EXISTS books_dapper_db_test;
        ";

        var connection = _context.CreateConnection();
        try
        {
            connection.Open();
            connection.QueryMultiple(query);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception executing EnsureDeleted(): {e}");
        }
        finally
        {
            connection.Close();
        }
    }

    public void EnsureCreated()
    {
        const string query = $@"
            CREATE DATABASE books_dapper_db_test;
        ";

        var connection = _context.CreateConnection();
        try
        {
            connection.Open();
            connection.QueryMultiple(query);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception executing EnsureCreated(): {e}");
        }
        finally
        {
            connection.Close();
        }
    }

    public void AddBooks(IEnumerable<Book> items)
    {
        const string query = $@"
            INSERT INTO {BookSchema.Table} ({BookSchema.Columns.Id}, {BookSchema.Columns.Title}, {BookSchema.Columns.PublishedOn})
            VALUES (@{nameof(Book.Id)}, @{nameof(Book.Title)}, @{nameof(Book.PublishedOn)})";

        var connection = _context.CreateConnection();
        try
        {
            connection.Open();
            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                parameters.Add(nameof(Book.Id), item.Id, DbType.Guid);
                parameters.Add(nameof(Book.Title), item.Title, DbType.String);
                parameters.Add(nameof(Book.PublishedOn), item.PublishedOn, DbType.DateTime);

                connection.Execute(query, parameters);
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
}