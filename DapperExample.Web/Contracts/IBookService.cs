using DapperExample.Web.DTOs;
using DapperExample.Web.Models;

namespace DapperExample.Web.Contracts;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAll();
    Task<BookDto?> GetById(Guid id);
    Task<BookBaseDto> Create(BookForCreatingDto item);
    Task<BookBaseDto> Update(BookForUpdatingDto item);
    Task Remove(Guid id);
}