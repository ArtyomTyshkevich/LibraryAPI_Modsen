namespace Library.Application.Interfaces
{

    public interface IRepository<T>
    {
        Task<List<T>> Get();
        Task<T?> Get(Guid id);
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}