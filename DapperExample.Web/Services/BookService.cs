using System.Data;
using Dapper;
using DapperExample.Web.Contracts;
using DapperExample.Web.Data;
using DapperExample.Web.Data.Schemas;
using DapperExample.Web.DTOs;
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

    public async Task<BookBaseDto> Create(BookForCreatingDto item)
    {
        var newItem = new Book()
        {
            Id = Guid.NewGuid(),
            Title = item.Title!,
            PublishedOn = item.PublishedOn!.Value.ToUniversalTime()
        };

        const string query = $@"
            INSERT INTO {BookSchema.Table} ({BookSchema.Columns.Id}, {BookSchema.Columns.Title}, {BookSchema.Columns.PublishedOn})
            VALUES (@{nameof(Book.Id)}, @{nameof(Book.Title)}, @{nameof(Book.PublishedOn)})";

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Book.Id), newItem.Id, DbType.Guid);
        parameters.Add(nameof(Book.Title), newItem.Title, DbType.String);
        parameters.Add(nameof(Book.PublishedOn), newItem.PublishedOn, DbType.DateTime);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, parameters);

        return new BookBaseDto
        {
            Id = newItem.Id,
            Title = newItem.Title,
            PublishedOn = newItem.PublishedOn
        };
    }

    public async Task<BookBaseDto> Update(BookForUpdatingDto item)
    {
        var itemToUpdate = new Book()
        {
            Id = item.Id!.Value,
            Title = item.Title!,
            PublishedOn = item.PublishedOn!.Value.ToUniversalTime()
        };

        const string query = $@"
            UPDATE {BookSchema.Table} SET 
              {BookSchema.Columns.Title} = @{nameof(Book.Title)}, 
              {BookSchema.Columns.PublishedOn} = @{nameof(Book.PublishedOn)} 
            WHERE {BookSchema.Columns.Id} = @{nameof(Book.Id)}";

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Book.Id), itemToUpdate.Id, DbType.Guid);
        parameters.Add(nameof(Book.Title), itemToUpdate.Title, DbType.String);
        parameters.Add(nameof(Book.PublishedOn), itemToUpdate.PublishedOn, DbType.DateTime);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, parameters);

        return new BookBaseDto
        {
            Id = itemToUpdate.Id,
            Title = itemToUpdate.Title,
            PublishedOn = itemToUpdate.PublishedOn
        };
    }

    public async Task Remove(Guid id)
    {
        const string query = $"DELETE FROM {BookSchema.Table} WHERE {BookSchema.Columns.Id} = @{nameof(Book.Id)}";

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Book.Id), id, DbType.Guid);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, parameters);

        /* try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ItemExists(id))
            {
                throw new EntityNotFoundException(id);
            }
            else
            {
                throw;
            }
        } */
    }

    private async Task<IEnumerable<BookDto>> GetItemData(Guid? itemId = null)
    {
        const string baseQuery = $@"
            SELECT b.{BookSchema.Columns.Id}, 
                   b.{BookSchema.Columns.Title}, 
                   b.{BookSchema.Columns.PublishedOn}, 
                   r.{ReviewSchema.Columns.Id}, 
                   r.{ReviewSchema.Columns.Comment}, 
                   r.{ReviewSchema.Columns.Rating} 
            FROM {BookSchema.Table} b 
            LEFT JOIN {ReviewSchema.Table} r ON b.{BookSchema.Columns.Id} = r.{ReviewSchema.Columns.BookId}";

        using var connection = _context.CreateConnection();
        var result = new Dictionary<Guid, BookDto>();
        await connection.QueryAsync<Book, Review, Book>(itemId == null ? baseQuery : $"{baseQuery} WHERE b.{BookSchema.Columns.Id} = '{itemId.Value}'",
            (book, review) =>
            {
                // Check if the item was already added
                if (result.TryGetValue(book.Id, out var existingItem))
                {
                    existingItem.Reviews.Add(new ReviewBaseDto {Id = review.Id, Comment = review.Comment, Rating = review.Rating});
                }
                else
                {
                    var newItem = new BookDto {Id = book.Id, Title = book.Title, PublishedOn = book.PublishedOn};
                    if (review != null)
                        newItem.Reviews.Add(new ReviewBaseDto {Id = review.Id, Comment = review.Comment, Rating = review.Rating});
                    result.Add(book.Id, newItem);
                }

                return book;
            }
        );

        return result.Values;
    }
}