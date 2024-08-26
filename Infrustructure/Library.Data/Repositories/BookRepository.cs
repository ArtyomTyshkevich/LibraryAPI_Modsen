using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Data.Repositories
{
    public class BookRepository : IRepository<Book>
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

        public async Task Create(Book Book)
        {
            await _libraryDbContext.Books.AddAsync(Book);
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task Delete(Book book)
        {
            _libraryDbContext.Books.Remove(book);
            if (book!.ImageFileName != null)
            {
                string OldImagePath = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!, book.ImageFileName);
                System.IO.File.Delete(OldImagePath);
            }
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task<List<Book>> Get()
        {
            return await _libraryDbContext.Books
                                            .Include(b=>b.Author)   
                                            .ToListAsync();
        }

        public async Task<Book?> Get(Guid userId)
        {
            return await _libraryDbContext.Books
                                            .Include(b=>b.Author)                       
                                            .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task Update(Book book)
        {
            _libraryDbContext.Books.Update(book);
            await _libraryDbContext.SaveChangesAsync();
        }
    }
}
