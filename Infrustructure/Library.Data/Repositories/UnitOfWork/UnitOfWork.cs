using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Interfaces;


namespace Library.Data.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _libraryDbContext;

        public UnitOfWork(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
            Authors = new AuthorRepository(_libraryDbContext);
            Books = new BookRepository(_libraryDbContext);
        }

        public IRepository<Author> Authors { get; private set; }
        public IRepository<Book> Books { get; private set; }

        public async Task<int> SaveChangesAsync() =>
            await _libraryDbContext.SaveChangesAsync();

        public void Dispose() => _libraryDbContext.Dispose();
    }
}
