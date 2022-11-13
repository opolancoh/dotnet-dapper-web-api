using System.Data;
using Dapper;
using DapperExample.Web.Contracts;
using DapperExample.Web.Data;
using DapperExample.Web.Data.Schemas;
using DapperExample.Web.DTOs;
using DapperExample.Web.Exceptions;
using DapperExample.Web.Models;

namespace DapperExample.Web.Services;

public class ReviewService : IReviewService
{
    private readonly IDapperContext _context;

    public ReviewService(IDapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReviewDto>> GetAll()
    {
        return await GetItemData();
    }

    public async Task<ReviewDto?> GetById(Guid id)
    {
        var result = await GetItemData(id);

        return result.SingleOrDefault();
    }

    public async Task<Guid> Create(ReviewForCreatingDto item)
    {
        var newItem = new Review
        {
            Id = Guid.NewGuid(),
            Comment = item.Comment!,
            Rating = item.Rating!.Value,
            BookId = item.BookId!.Value
        };

        const string query = $@"
            INSERT INTO {ReviewSchema.Table} (
              {ReviewSchema.Columns.Id},
              {ReviewSchema.Columns.Comment},
              {ReviewSchema.Columns.Rating},
              {ReviewSchema.Columns.BookId}
            )
            VALUES (
                @{nameof(Review.Id)}, 
                @{nameof(Review.Comment)}, 
                @{nameof(Review.Rating)},
                @{nameof(Review.BookId)}
            )
        ";

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Review.Id), newItem.Id, DbType.Guid);
        parameters.Add(nameof(Review.Comment), newItem.Comment, DbType.String);
        parameters.Add(nameof(Review.Rating), newItem.Rating, DbType.Int32);
        parameters.Add(nameof(Review.BookId), newItem.BookId, DbType.Guid);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(query, parameters);

        if (result == 0)
            throw new Exception("The resource was not modified.");

        return newItem.Id;
    }

    public async Task Update(ReviewForUpdatingDto item)
    {
        var itemToUpdate = new Review()
        {
            Id = item.Id!.Value,
            Comment = item.Comment!,
            Rating = item.Rating!.Value
        };

        const string query = $@"
            UPDATE {ReviewSchema.Table} SET 
              {ReviewSchema.Columns.Comment} = @{nameof(Review.Comment)}, 
              {ReviewSchema.Columns.Rating} = @{nameof(Review.Rating)}              
            WHERE {ReviewSchema.Columns.Id} = @{nameof(Review.Id)}";

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Review.Id), itemToUpdate.Id, DbType.Guid);
        parameters.Add(nameof(Review.Comment), itemToUpdate.Comment, DbType.String);
        parameters.Add(nameof(Review.Rating), itemToUpdate.Rating, DbType.Int32);

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
        const string query = $"DELETE FROM {ReviewSchema.Table} WHERE {ReviewSchema.Columns.Id} = @{nameof(Book.Id)}";

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Book.Id), id, DbType.Guid);

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
            $"SELECT COUNT(1) FROM {ReviewSchema.Table} WHERE {ReviewSchema.Columns.Id} = '{id}'");
    }

    private async Task<IEnumerable<ReviewDto>> GetItemData(Guid? itemId = null)
    {
        const string baseQuery = $@"
            SELECT r.{ReviewSchema.Columns.Id}, 
                   r.{ReviewSchema.Columns.Comment}, 
                   r.{ReviewSchema.Columns.Rating}, 
                   r.{ReviewSchema.Columns.BookId} 
            FROM {ReviewSchema.Table} r
         ";

        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<ReviewDto>(
            itemId == null ? baseQuery : $"{baseQuery} WHERE r.{ReviewSchema.Columns.Id} = '{itemId.Value}'"
        );

        return result;
    }
}