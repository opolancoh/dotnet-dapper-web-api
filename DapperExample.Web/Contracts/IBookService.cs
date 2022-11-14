using DapperExample.Web.DTOs;
using DapperExample.Web.Models;

namespace DapperExample.Web.Contracts;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAll();
    Task<BookDto?> GetById(Guid id);
    Task<Guid> Create(BookForCreatingDto item);
    Task Update(BookForUpdatingDto item);
    Task Remove(Guid id);
}