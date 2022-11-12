namespace DapperExample.Web.Models;

public class Book
{
    private readonly DateTime _publishedOn;

    public Guid Id { get; set; }
    public string Title { get; set; }

    public DateTime PublishedOn
    {
        get => _publishedOn;
        init => _publishedOn = value.ToUniversalTime();
    }

    // One-to-many relationship (Review)
    // public ICollection<Review> Reviews { get; set; }
    public List<Review> Reviews { get; set; } = new List<Review>();
}