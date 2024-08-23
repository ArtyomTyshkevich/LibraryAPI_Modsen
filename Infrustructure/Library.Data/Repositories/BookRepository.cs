using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class BookRepository : IRepository<Book>
    {
        private readonly LibraryDbContext _libraryDbContext;
        public BookRepository(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        public async Task Create(Book Book)
        {
            await _libraryDbContext.Books.AddAsync(Book);
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task Delete(Book book)
        {
            _libraryDbContext.Books.Remove(book);
            string solutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            if (book!.ImageFileName != null)
            {
                string OldImagePath = Path.Combine(solutionPath, "Library API", "Infrustructure", "Library.Data", "FileStorage", "BookImages", book.ImageFileName);
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
