using DapperExample.Web.Models;

namespace DapperExample.Web.Data.Schemas;

public static class BookSchema
{
    public const string Table = $"{nameof(Book)}s";

    public static class Columns
    {
        public const string Id = $"{nameof(Book.Id)}";
        public const string Title = $"{nameof(Book.Title)}";
        public const string PublishedOn = $"{nameof(Book.PublishedOn)}";
    }
}