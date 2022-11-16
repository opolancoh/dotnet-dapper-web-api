using System.Data;
using Dapper;
using DapperExample.Web.Contracts;
using DapperExample.Web.Data;
using DapperExample.Web.Data.DatabaseContext;
using DapperExample.Web.Data.Schemas;
using DapperExample.Web.DTOs;
using DapperExample.Web.Exceptions;
using DapperExample.Web.Models;

namespace DapperExample.Web.Services;

public class BookService : IBookService
{
    private readonly DapperContext _context;

    public BookService(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookDto>> GetAll()
    {
        return await GetItemData();
    }

    public async Task<BookDto?> GetById(Guid id)
    {
        var result = await GetItemData(id);

        return result.SingleOrDefault();
    }

    public async Task<Guid> Create(BookForCreatingDto item)
    {
        var newItem = new Book()
        {
            Id = Guid.NewGuid(),
            Title = item.Title!,
            PublishedOn = item.PublishedOn!.Value
        };

        const string query =
            $@"INSERT INTO ""{BookSchema.Table}"" (""{BookSchema.Columns.Id}"", ""{BookSchema.Columns.Title}"", ""{BookSchema.Columns.PublishedOn}"") " +
            $@"VALUES (@{BookSchema.Columns.Id}, @{BookSchema.Columns.Title}, @{BookSchema.Columns.PublishedOn})";

        var parameters = new DynamicParameters();
        parameters.Add(BookSchema.Columns.Id, newItem.Id, DbType.Guid);
        parameters.Add(BookSchema.Columns.Title, newItem.Title, DbType.String);
        parameters.Add(BookSchema.Columns.PublishedOn, newItem.PublishedOn, DbType.DateTime);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(query, parameters);

        if (result == 0)
            throw new Exception("The resource was not modified.");

        return newItem.Id;
    }

    public async Task Update(BookForUpdatingDto item)
    {
        var itemToUpdate = new Book()
        {
            Id = item.Id!.Value,
            Title = item.Title!,
            PublishedOn = item.PublishedOn!.Value.ToUniversalTime()
        };

        const string query =
            $@"UPDATE ""{BookSchema.Table}"" SET " +
            $@"""{BookSchema.Columns.Title}"" = @{BookSchema.Columns.Title}, " +
            $@"""{BookSchema.Columns.PublishedOn}"" = @{BookSchema.Columns.PublishedOn} " +
            $@"WHERE ""{BookSchema.Columns.Id}"" = @{BookSchema.Columns.Id}";

        var parameters = new DynamicParameters();
        parameters.Add(BookSchema.Columns.Id, itemToUpdate.Id, DbType.Guid);
        parameters.Add(BookSchema.Columns.Title, itemToUpdate.Title, DbType.String);
        parameters.Add(BookSchema.Columns.PublishedOn, itemToUpdate.PublishedOn, DbType.DateTime);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(query, parameters);

        if (result == 0)
        {
            var itemExists = await ItemExists(itemToUpdate.Id, connection);
            if (!itemExists)
                throw new EntityNotFoundException(itemToUpdate.Id);
            else
                throw new Exception("The resource was not modified.");
        }
    }

    public async Task Remove(Guid id)
    {
        const string query =
            $@"DELETE FROM ""{BookSchema.Table}"" WHERE ""{BookSchema.Columns.Id}"" = @{BookSchema.Columns.Id}";

        var parameters = new DynamicParameters();
        parameters.Add(BookSchema.Columns.Id, id, DbType.Guid);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(query, parameters);

        if (result == 0)
        {
            var itemExists = await ItemExists(id, connection);
            if (!itemExists)
                throw new EntityNotFoundException(id);
            else
                throw new Exception("The resource was not modified.");
        }
    }

    private async Task<bool> ItemExists(Guid id, IDbConnection connection)
    {
        return await connection.ExecuteScalarAsync<bool>(
            $@"SELECT COUNT(1) FROM ""{BookSchema.Table}"" WHERE ""{BookSchema.Columns.Id}"" = '{id}'");
    }

    private async Task<IEnumerable<BookDto>> GetItemData(Guid? itemId = null)
    {
        const string baseQuery =
            $@"SELECT " +
            $@"b.""{BookSchema.Columns.Id}"", " +
            $@"b.""{BookSchema.Columns.Title}"", " +
            $@"b.""{BookSchema.Columns.PublishedOn}"", " +
            $@"r.""{ReviewSchema.Columns.Id}"", " +
            $@"r.""{ReviewSchema.Columns.Comment}"", " +
            $@"r.""{ReviewSchema.Columns.Rating}"" " +
            $@"FROM ""{BookSchema.Table}"" b " +
            $@"LEFT JOIN ""{ReviewSchema.Table}"" r ON b.""{BookSchema.Columns.Id}"" = r.""{ReviewSchema.Columns.BookId}""";

        using var connection = _context.CreateConnection();
        var result = new Dictionary<Guid, BookDto>();
        await connection.QueryAsync<Book, Review, Book>(
            itemId == null ? baseQuery : $@"{baseQuery} WHERE b.""{BookSchema.Columns.Id}"" = '{itemId.Value}'",
            (book, review) =>
            {
                // Check if the item was already added
                if (result.TryGetValue(book.Id, out var existingItem))
                {
                    existingItem.Reviews.Add(new ReviewBaseDto
                        { Id = review.Id, Comment = review.Comment, Rating = review.Rating });
                }
                else
                {
                    var newItem = new BookDto { Id = book.Id, Title = book.Title, PublishedOn = book.PublishedOn };
                    if (review != null)
                        newItem.Reviews.Add(new ReviewBaseDto
                            { Id = review.Id, Comment = review.Comment, Rating = review.Rating });
                    result.Add(book.Id, newItem);
                }

                return book;
            }
        );

        return result.Values;
    }
}