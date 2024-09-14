using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class AuthorRepository : IRepository<Author, Guid>
    {
        private readonly LibraryDbContext _libraryDbContext;

        public AuthorRepository(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        public async Task Create(Author author, CancellationToken cancellationToken)
        {
            await _libraryDbContext.Authors.AddAsync(author, cancellationToken);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Author author, CancellationToken cancellationToken)
        {
            _libraryDbContext.Authors.Remove(author);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Author>> Get(CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Authors.ToListAsync(cancellationToken);
        }

        public async Task<Author?> Get(Guid authorId, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Authors
                                           .Include(aut => aut.Books)
                                           .FirstAsync(a => a.Id == authorId, cancellationToken);
        }

        public async Task Update(Author author, CancellationToken cancellationToken)
        {
            _libraryDbContext.Authors.Update(author);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Author>> GetWithPagination(int pageNum, int pageSize, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Authors
                                           .Skip((pageNum - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync(cancellationToken);
        }
    }
}
