
namespace Library.Application.Interfaces
{
    public interface IBookRepository<T> : IRepository<T, Guid>
    {
        Task<T> GetByISBN(string ISBN, CancellationToken cancellationToken);
    }
}