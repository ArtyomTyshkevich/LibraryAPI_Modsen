using Library.Domain.Entities;

namespace Library.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Author> Authors { get; }
        IRepository<Book> Books { get; }
        Task<int> SaveChangesAsync();
    }
}