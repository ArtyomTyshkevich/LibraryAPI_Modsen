
using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Interfaces
{
    public interface IUserRepository<T> : IRepository<T, long>
    {
        Task<List<long>> GetRolesId(T user, CancellationToken cancellationToken);
        Task<List<IdentityRole<long>>> GetRoles(T entity, CancellationToken cancellationToken);
        Task<T> GetByMail(string mail, CancellationToken cancellationToken);
    }
}