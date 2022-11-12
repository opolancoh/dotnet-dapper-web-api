using System.ComponentModel.DataAnnotations;

namespace DapperExample.Web.DTOs;

public record BookForUpdatingDto : BookForCreatingDto
{
    [Required] public Guid? Id { get; init; }
}
