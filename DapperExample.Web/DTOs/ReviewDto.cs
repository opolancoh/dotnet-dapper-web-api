using System.ComponentModel.DataAnnotations;
using DapperExample.Web.Data.Validations;

namespace DapperExample.Web.DTOs;

public record ReviewBaseDto
{
    public Guid Id { get; init; }
    public string Comment { get; init; }
    public int Rating { get; init; }
};

public record ReviewDto : ReviewBaseDto
{
    public Guid BookId { get; init; }
};

public record ReviewForCreatingDto : IValidatableObject
{
    [Required] public string Comment { get; init; }
    [Required] public int? Rating { get; init; }

    [Required] public Guid? BookId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        validationResult.AddRange(ReviewValidation.ValidateRating(Rating!.Value, nameof(Rating)));

        return validationResult;
    }
};

public record ReviewForUpdatingDto : IValidatableObject
{
    [Required] public Guid? Id { get; init; }
    [Required] public string Comment { get; init; }
    [Required] public int? Rating { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        validationResult.AddRange(ReviewValidation.ValidateRating(Rating!.Value, nameof(Rating)));

        return validationResult;
    }
};