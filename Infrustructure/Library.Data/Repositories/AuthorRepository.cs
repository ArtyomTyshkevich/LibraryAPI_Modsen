using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Library.Data.Repositories
{
    public class AuthorRepository : IRepository<Author>
    {
        private readonly LibraryDbContext _libraryDbContext;

        public AuthorRepository(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        public async Task Create(Author author)
        {
            await _libraryDbContext.Authors.AddAsync(author);
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task Delete(Author author)
        {
            _libraryDbContext.Authors.Remove(author);
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task<List<Author>> Get()
        {
            return await _libraryDbContext.Authors.ToListAsync();
        }

        public async Task<Author> Get(Guid authorId)
        {
            return await _libraryDbContext.Authors
                                           .Include(aut=>aut.Books)
                                           .FirstAsync(a => a.Id == authorId);
        }

        public async Task Update(Author author)
        {
            _libraryDbContext.Authors.Update(author);
            await _libraryDbContext.SaveChangesAsync();
        }
    }
}