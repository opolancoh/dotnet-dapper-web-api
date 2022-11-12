using System.ComponentModel.DataAnnotations;
using DapperExample.Web.Data.Validations;

namespace DapperExample.Web.DTOs;

public record BookForCreatingDto : IValidatableObject
{
    [Required] public string? Title { get; init; }
    [Required] public DateTime? PublishedOn { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        validationResult.AddRange(BookValidation.ValidateTitle(Title!, nameof(Title)));

        return validationResult;
    }
};