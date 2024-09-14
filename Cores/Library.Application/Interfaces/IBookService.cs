using Library.Application.DTOs;
using Library.Domain.Entities;

namespace Library.Application.Interfaces
{
    public interface IBookService
    {
        Task AddImageToBook(BookDTO bookDTO, CancellationToken cancellationToken = default);
        Task<string> AddBookToUser(Guid bookId, long userId, CancellationToken cancellationToken = default);
        Task<BookDTO> BooksByIdRedis(Guid id, CancellationToken cancellationToken = default);
        Task<BookDTO> BooksByISBNFileSystem(string ISBN, CancellationToken cancellationToken = default);
        Task<Book?> BookToUser(Guid bookId, long userId, CancellationToken cancellationToken = default);
    }
}
