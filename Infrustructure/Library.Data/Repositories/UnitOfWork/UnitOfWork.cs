using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Library.Data.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public UnitOfWork(LibraryDbContext libraryDbContext, IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _libraryDbContext = libraryDbContext;
            _configuration = configuration;
            Authors = new AuthorRepository(_libraryDbContext);
            Books = new BookRepository(_libraryDbContext, _env, _configuration);
            Users = new UserRepository(_libraryDbContext);
        }

        public IUserRepository<User> Users { get; private set; }
        public IRepository<Author, Guid> Authors { get; private set; }
        public IBookRepository<Book> Books { get; private set; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await _libraryDbContext.SaveChangesAsync(cancellationToken);

        public void Dispose() => _libraryDbContext.Dispose();
    }
}