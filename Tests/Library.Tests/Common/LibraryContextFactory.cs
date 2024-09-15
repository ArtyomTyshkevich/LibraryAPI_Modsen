using Microsoft.EntityFrameworkCore;
using Library.Data.Context;

namespace Library.Tests.Common
{
    public class LibraryContextFactory
    {
        public static LibraryDbContext CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LibraryDbContext(options);
        }

        public static void Destroy(LibraryDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}