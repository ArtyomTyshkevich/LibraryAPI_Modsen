using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Data.Repositories
{
    public class BookRepository : IBookRepository<Book>
    {
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public BookRepository(LibraryDbContext libraryDbContext, IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _libraryDbContext = libraryDbContext;
            _configuration = configuration;
        }

        public async Task Create(Book book, CancellationToken cancellationToken)
        {
            await _libraryDbContext.Books.AddAsync(book, cancellationToken);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Book book, CancellationToken cancellationToken)
        {
            _libraryDbContext.Books.Remove(book);
            if (book!.ImageFileName != null)
            {
                string oldImagePath = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!, book.ImageFileName);
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Book>> Get(CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Books
                                           .Include(b => b.Author)
                                           .ToListAsync(cancellationToken);
        }

        public async Task<Book?> Get(Guid bookId, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Books
                                           .Include(b => b.Author)
                                           .FirstOrDefaultAsync(x => x.Id == bookId, cancellationToken);
        }

        public async Task<Book> GetByISBN(string isbn, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Books
                                          .Include(b => b.Author)
                                          .FirstAsync(a => a.ISBN == isbn, cancellationToken);
        }

        public async Task<List<Book>> GetWithPagination(int pageNum, int pageSize, CancellationToken cancellationToken)
        {
            return await _libraryDbContext.Books
                                           .Skip((pageNum - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync(cancellationToken);
        }

        public async Task Update(Book book, CancellationToken cancellationToken)
        {
            _libraryDbContext.Books.Update(book);
            await _libraryDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}