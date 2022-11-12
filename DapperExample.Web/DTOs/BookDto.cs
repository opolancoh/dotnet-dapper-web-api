namespace DapperExample.Web.DTOs;

public record BookBaseDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public DateTime PublishedOn { get; init; }
};

public record BookDto : BookBaseDto
{
    public ICollection<ReviewBaseDto> Reviews { get; init; } = new List<ReviewBaseDto>();
};