using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Library.Data.Repositories
{
    public class UserRepository : IUserRepository<User>
    {
        private readonly LibraryDbContext _libraryDbContext;

        public UserRepository(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        public async Task Create(User user, CancellationToken cancellationToken)
        {
            await _libraryDbContext.Users.AddAsync(user, cancellationToken);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(User user, CancellationToken cancellationToken)
        {
            _libraryDbContext.Users.Remove(user);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<User>> Get(CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Users
                                            .Include(b => b.UserName)
                                            .ToListAsync(cancellationToken);
        }

        public async Task<User?> Get(long userId, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Users
                                 .Include(u => u.Books)
                                 .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }
        public async Task<List<long>> GetRolesId(User user, CancellationToken cancellationToken)
        {
            var roleIds = await _libraryDbContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            return roleIds;
        }

        public async Task<List<IdentityRole<long>>> GetRoles(User entity, CancellationToken cancellationToken)
        {
            var roleIds = await GetRolesId(entity, cancellationToken);

            var roles = await _libraryDbContext.Roles
                .Where(role => roleIds.Contains(role.Id))
                .ToListAsync(cancellationToken);

            return roles;
        }

        public async Task<List<User>> GetWithPagination(int pageNum, int pageSize, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Users
                                           .Skip((pageNum - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync(cancellationToken);
        }

        public async Task Update(User user, CancellationToken cancellationToken)
        {
            _libraryDbContext.Users.Update(user);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<User> GetByMail(string mail, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Users
                .FirstAsync(u => u.Email == mail, cancellationToken);
        }
    }
}
