using DapperExample.Web.Models;

namespace DapperExample.Tests.Helpers;

public static class DbDataHelper
{
    public static Guid BookId1 = new Guid("01692cba-f59b-4f02-9ee2-fcbbb04a7b29");
    public static Guid BookId2 = new Guid("9c6ad8fd-1837-4b64-ab84-5a42de5b8529");
    public static Guid BookId3 = new Guid("cff5860a-3c38-4d0d-966c-770b1bfaeb92");
    public static Guid BookId4 = new Guid("d5c8df05-64be-4d36-8ca7-461242542873");
    public static Guid BookId5 = new Guid("dae1cc58-84ed-4e5f-a8ad-e7714fbea7ac");

    public static Guid ReviewId1 = new Guid("1110e66a-8b41-4d6e-9f63-f00e881d08c7");
    public static Guid ReviewId2 = new Guid("223c62a8-13b7-4862-8dbf-b52442a17d1f");
    public static Guid ReviewId3 = new Guid("3fad3762-e3eb-4656-9c0b-db9b0814a0fe");
    public static Guid ReviewId4 = new Guid("4be241c4-1f5e-471e-a5fa-16f28701d2d1");
    public static Guid ReviewId5 = new Guid("9b81e4b7-9e4d-41ed-8a90-b1df20ca49a7");
    public static Guid ReviewId6 = new Guid("b79209f6-a129-4968-9c15-d8327e81ee51");
    public static Guid ReviewId7 = new Guid("cb808930-c118-4bb6-95d5-d8b4f7d41e75");
    public static Guid ReviewId8 = new Guid("df49dc81-da48-40a8-9bbd-d992ac98aa54");


    public static List<Book> Books =>
        new List<Book>
        {
            new()
            {
                Id = BookId1,
                Title = "Book 101",
                PublishedOn = new DateTime(2022, 01, 24).ToUniversalTime(),
            },
            new()
            {
                Id = BookId2,
                Title = "Book 102",
                PublishedOn = new DateTime(2022, 01, 19).ToUniversalTime(),
            },
            new()
            {
                Id = BookId3,
                Title = "Book 203",
                PublishedOn = new DateTime(2022, 04, 26).ToUniversalTime(),
            },
            new()
            {
                Id = BookId4,
                Title = "Book 204",
                PublishedOn = new DateTime(2022, 07, 13).ToUniversalTime(),
            },
            new()
            {
                Id = BookId5,
                Title = "Book 305",
                PublishedOn = new DateTime(2022, 10, 20).ToUniversalTime(),
            },
        };

    public static List<Review> Reviews =>
        new List<Review>
        {
            new()
            {
                Id = ReviewId1,
                Comment = "Comment 101",
                Rating = 1,
                BookId = BookId1
            },
            new()
            {
                Id = ReviewId2,
                Comment = "Comment 102",
                Rating = 2,
                BookId = BookId1
            },
            new()
            {
                Id = ReviewId3,
                Comment = "Comment 203",
                Rating = 3,
                BookId = BookId2
            },
            new()
            {
                Id = ReviewId4,
                Comment = "Comment 204",
                Rating = 4,
                BookId = BookId2
            },
            new()
            {
                Id = ReviewId5,
                Comment = "Comment 205",
                Rating = 5,
                BookId = BookId3
            },
            new()
            {
                Id = ReviewId6,
                Comment = "Comment 306",
                Rating = 1,
                BookId = BookId3
            },
            new()
            {
                Id = ReviewId7,
                Comment = "Comment 407",
                Rating = 2,
                BookId = BookId3
            },
            new()
            {
                Id = ReviewId8,
                Comment = "Comment 408",
                Rating = 3,
                BookId = BookId3
            }
        };
}