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
            Books = new BookRepository(_libraryDbContext,_env,_configuration);
        }

        public IRepository<Author> Authors { get; private set; }
        public IRepository<Book> Books { get; private set; }

        public async Task<int> SaveChangesAsync() =>
            await _libraryDbContext.SaveChangesAsync();

        public void Dispose() => _libraryDbContext.Dispose();
    }
}
