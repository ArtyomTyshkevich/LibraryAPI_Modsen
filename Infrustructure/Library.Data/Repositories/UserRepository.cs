using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class UserRepository : IRepository<User, long>
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
    }
}
