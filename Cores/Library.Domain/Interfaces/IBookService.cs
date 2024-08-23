
using Library.Domain.DTOs;
using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    public interface IBookService
    {
        Task AddImageToBook(BookDTO BookDTO);
        Task<string> AddBookToUser(Guid bookId, string userId);
        Task<List<Book>> BooksPagination(int pageNum, int pageSize);
    }
}