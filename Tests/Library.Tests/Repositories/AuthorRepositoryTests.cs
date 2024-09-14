//using Library.Data.Repositories;
//using Library.Domain.Entities;
//using Library.Tests.Common;
//using Microsoft.EntityFrameworkCore;

//namespace Library.Tests.Repositories
//{
//    public class AuthorRepositoryTests : TestCommandBase
//    {
//        [Fact]
//        public async Task CreateAuthor_Success()
//        {
//            // Arrange
//            var authorId = Guid.Parse("77777777-7777-7777-7777-777777777771");
//            var author = new Author
//            {
//                Id = authorId,
//                Name = "Test Author",
//                Birthday = DateTime.MinValue,
//                Country = "Belarus"
//            };
//            var authorRepository = new AuthorRepository(Context);

//            // Act
//            await authorRepository.Create(author);
//            await Context.SaveChangesAsync();

//            // Assert
//            var createdAuthor = await Context.Authors.FirstOrDefaultAsync(aut =>
//                aut.Id == author.Id &&
//                aut.Name == author.Name &&
//                aut.Birthday == author.Birthday &&
//                aut.Country == author.Country);

//            Assert.NotNull(createdAuthor);
//        }

//        [Fact]
//        public async Task GetAuthorById_Success()
//        {
//            // Arrange
//            var authorId = Guid.Parse("77777777-7777-7777-7777-777777777772");
//            var author = new Author
//            {
//                Id = authorId,
//                Name = "Test Author",
//                Birthday = DateTime.MinValue,
//                Country = "Belarus"
//            };
//            await Context.Authors.AddAsync(author);
//            await Context.SaveChangesAsync();
//            var authorRepository = new AuthorRepository(Context);

//            // Act
//            var result = await authorRepository.Get(authorId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(authorId, result.Id);
//            Assert.Equal("Test Author", result.Name);
//            Assert.Equal(DateTime.MinValue, result.Birthday);
//            Assert.Equal("Belarus", result.Country);
//        }

//        [Fact]
//        public async Task DeleteAuthorById_Success()
//        {
//            // Arrange
//            var authorId = Guid.Parse("77777777-7777-7777-7777-777777777773");
//            var author = new Author
//            {
//                Id = authorId,
//                Name = "Test Author",
//                Birthday = DateTime.MinValue,
//                Country = "Belarus"
//            };
//            await Context.Authors.AddAsync(author);
//            await Context.SaveChangesAsync();
//            var authorRepository = new AuthorRepository(Context);

//            // Act
//            await authorRepository.Delete(author);
//            await Context.SaveChangesAsync();

//            // Assert
//            var result = await Context.Authors.FindAsync(authorId);
//            Assert.Null(result);
//        }

//        [Fact]
//        public async Task DeleteAuthorById_Unsuccess()
//        {
//            // Arrange
//            var nonExistentAuthorId = Guid.NewGuid();
//            var author = new Author
//            {
//                Id = nonExistentAuthorId,
//                Name = "Non-Existent Author",
//                Birthday = DateTime.MinValue,
//                Country = "Unknown"
//            };
//            var authorRepository = new AuthorRepository(Context);

//            // Act & Assert
//            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
//            {
//                await authorRepository.Delete(author);
//                await Context.SaveChangesAsync();
//            });
//        }

//        [Fact]
//        public async Task UpdateAuthor_Success()
//        {
//            // Arrange
//            var author = await Context.Authors.FindAsync(LibraryContextFactory.AuthorAId);
//            author!.Name = "Updated Author";
//            author.Birthday = new DateTime(1990, 1, 1);
//            author.Country = "Netherlands";
//            var authorRepository = new AuthorRepository(Context);

//            // Act
//            await authorRepository.Update(author);
//            await Context.SaveChangesAsync();

//            // Assert
//            var updatedAuthor = await Context.Authors.FindAsync(LibraryContextFactory.AuthorAId);
//            Assert.NotNull(updatedAuthor);
//            Assert.Equal("Updated Author", updatedAuthor.Name);
//            Assert.Equal(new DateTime(1990, 1, 1), updatedAuthor.Birthday);
//            Assert.Equal("Netherlands", updatedAuthor.Country);
//        }

//        [Fact]
//        public async Task UpdateAuthor_Unsuccess()
//        {
//            // Arrange
//            var nonExistentAuthorId = Guid.NewGuid();
//            var author = new Author
//            {
//                Id = nonExistentAuthorId,
//                Name = "Non-Existent Author",
//                Birthday = DateTime.MinValue,
//                Country = "Unknown"
//            };
//            var authorRepository = new AuthorRepository(Context);

//            // Act & Assert
//            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
//            {
//                await authorRepository.Update(author);
//                await Context.SaveChangesAsync();
//            });
//        }
//    }
//}