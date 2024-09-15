using Library.Data.Repositories;
using Library.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests.Repositories
{
    public class AuthorRepositoryTests : TestCommandBase
    {
        [Fact]
        public async Task CreateAuthor_Success()
        {
            // Arrange
            var author = AuthorTestData.CreateTestAuthor();
            var authorRepository = new AuthorRepository(Context);

            // Act
            await authorRepository.Create(author, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var createdAuthor = await Context.Authors.FirstOrDefaultAsync(aut =>
                aut.Id == author.Id &&
                aut.Name == author.Name &&
                aut.Birthday == author.Birthday &&
                aut.Country == author.Country);

            Assert.NotNull(createdAuthor);
        }

        [Fact]
        public async Task GetAuthors_Success()
        {
            // Arrange
            var author1 = AuthorTestData.CreateTestAuthor();
            var author2 = AuthorTestData.CreateTestAuthor();
            await Context.Authors.AddRangeAsync(author1, author2);
            await Context.SaveChangesAsync();
            var authorRepository = new AuthorRepository(Context);

            // Act
            var authors = await authorRepository.Get(CancellationToken.None);

            // Assert
            Assert.NotNull(authors);
            Assert.Equal(2, authors.Count);
        }

        [Fact]
        public async Task GetAuthorById_Success()
        {
            // Arrange
            var author = AuthorTestData.CreateTestAuthor();
            await Context.Authors.AddAsync(author);
            await Context.SaveChangesAsync();
            var authorRepository = new AuthorRepository(Context);

            // Act
            var result = await authorRepository.Get(author.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(author.Id, result.Id);
            Assert.Equal(author.Name, result.Name);
        }

        [Fact]
        public async Task DeleteAuthor_Success()
        {
            // Arrange
            var author = AuthorTestData.CreateTestAuthor();
            await Context.Authors.AddAsync(author);
            await Context.SaveChangesAsync();
            var authorRepository = new AuthorRepository(Context);

            // Act
            await authorRepository.Delete(author, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var deletedAuthor = await Context.Authors.FindAsync(author.Id);
            Assert.Null(deletedAuthor);
        }

        [Fact]
        public async Task UpdateAuthorSuccess()
        {
            // Arrange
            var author = AuthorTestData.CreateTestAuthor();
            await Context.Authors.AddAsync(author);
            await Context.SaveChangesAsync();
            var authorRepository = new AuthorRepository(Context);

            // Act
            author.Name = "Updated Name";
            await authorRepository.Update(author, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var updatedAuthor = await Context.Authors.FindAsync(author.Id);
            Assert.NotNull(updatedAuthor);
            Assert.Equal("Updated Name", updatedAuthor.Name);
        }

        [Fact]
        public async Task GetAuthorsWithPagination_Success()
        {
            // Arrange
            var author1 = AuthorTestData.CreateTestAuthor();
            var author2 = AuthorTestData.CreateTestAuthor();
            await Context.Authors.AddRangeAsync(author1, author2);
            await Context.SaveChangesAsync();
            var authorRepository = new AuthorRepository(Context);

            // Act
            var authorsPage = await authorRepository.GetWithPagination(1, 1, CancellationToken.None);

            // Assert
            Assert.Single(authorsPage);
            Assert.Contains(authorsPage, a => a.Id == author1.Id || a.Id == author2.Id);
        }
    }
}
