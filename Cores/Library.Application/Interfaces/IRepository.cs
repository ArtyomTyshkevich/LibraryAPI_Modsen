using Library.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Application.Interfaces
{
    public interface IRepository<T, TId>
    {
        Task<List<T>> Get(CancellationToken cancellationToken = default);
        Task<T?> Get(TId id, CancellationToken cancellationToken = default);
        Task Create(T entity, CancellationToken cancellationToken = default);
        Task Update(T entity, CancellationToken cancellationToken = default);
        Task Delete(T entity, CancellationToken cancellationToken = default);
        Task<List<T>> GetWithPagination(int pageNum, int pageSize, CancellationToken cancellationToken = default);
    }
}