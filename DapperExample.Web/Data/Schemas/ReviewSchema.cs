using DapperExample.Web.Models;

namespace DapperExample.Web.Data.Schemas;

public static class ReviewSchema
{
    public const string Table = $"\"{nameof(Review)}s\"";

    public static class Columns
    {
        public const string Id = $"\"{nameof(Review.Id)}\"";
        public const string Comment = $"\"{nameof(Review.Comment)}\"";
        public const string Rating = $"\"{nameof(Review.Rating)}\"";
        public const string BookId = $"\"{nameof(Review.BookId)}\"";
    }
}