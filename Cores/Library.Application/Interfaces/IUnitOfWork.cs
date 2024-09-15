using Library.Domain.Entities;

namespace Library.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository<User> Users { get; }
        IRepository<Author, Guid> Authors { get; }
        IBookRepository<Book> Books { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}