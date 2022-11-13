using DapperExample.Web.DTOs;
using DapperExample.Web.Models;

namespace DapperExample.Web.Contracts;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAll();
    Task<ReviewDto?> GetById(Guid id);
    Task<Guid> Create(ReviewForCreatingDto item);
    Task Update(ReviewForUpdatingDto item);
    Task Remove(Guid id);
}