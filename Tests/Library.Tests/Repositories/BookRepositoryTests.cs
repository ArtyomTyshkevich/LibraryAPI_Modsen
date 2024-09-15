using Library.Data.Repositories;
using Library.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace Library.Tests.Repositories
{
    public class BookRepositoryTests : TestCommandBase
    {
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public BookRepositoryTests()
        {
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Настраиваем фиктивный путь для хранения изображений
            _mockConfiguration.SetupGet(c => c["ImageStorage:Path"]).Returns("TestImages");
        }

        [Fact]
        public async Task CreateBook_Success()
        {
            // Arrange
            var book = BookTestData.CreateTestBook();
            var bookRepository = new BookRepository(Context, _mockWebHostEnvironment.Object, _mockConfiguration.Object);

            // Act
            await bookRepository.Create(book, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var createdBook = await Context.Books.FirstOrDefaultAsync(b =>
                b.Id == book.Id &&
                b.Name == book.Name &&
                b.ISBN == book.ISBN);

            Assert.NotNull(createdBook);
        }

        [Fact]
        public async Task GetBooks_Success()
        {
            // Arrange
            var book1 = BookTestData.CreateTestBook();
            var book2 = BookTestData.CreateTestBook();
            await Context.Books.AddRangeAsync(book1, book2);
            await Context.SaveChangesAsync();
            var bookRepository = new BookRepository(Context, _mockWebHostEnvironment.Object, _mockConfiguration.Object);

            // Act
            var books = await bookRepository.Get(CancellationToken.None);

            // Assert
            Assert.NotNull(books);
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public async Task GetBookById_Success()
        {
            // Arrange
            var book = BookTestData.CreateTestBook();
            await Context.Books.AddAsync(book);
            await Context.SaveChangesAsync();
            var bookRepository = new BookRepository(Context, _mockWebHostEnvironment.Object, _mockConfiguration.Object);

            // Act
            var result = await bookRepository.Get(book.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Id, result!.Id);
            Assert.Equal(book.Name, result.Name);
        }

        [Fact]
        public async Task UpdateBook_Success()
        {
            // Arrange
            var book = BookTestData.CreateTestBook();
            await Context.Books.AddAsync(book);
            await Context.SaveChangesAsync();
            var bookRepository = new BookRepository(Context, _mockWebHostEnvironment.Object, _mockConfiguration.Object);

            // Act
            book.Name = "Updated Book Name";
            await bookRepository.Update(book, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var updatedBook = await Context.Books.FindAsync(book.Id);
            Assert.NotNull(updatedBook);
            Assert.Equal("Updated Book Name", updatedBook!.Name);
        }

        [Fact]
        public async Task GetBooksWithPagination_Success()
        {
            // Arrange
            var book1 = BookTestData.CreateTestBook();
            var book2 = BookTestData.CreateTestBook();
            await Context.Books.AddRangeAsync(book1, book2);
            await Context.SaveChangesAsync();
            var bookRepository = new BookRepository(Context, _mockWebHostEnvironment.Object, _mockConfiguration.Object);

            // Act
            var booksPage = await bookRepository.GetWithPagination(1, 1, CancellationToken.None);

            // Assert
            Assert.Single(booksPage);
            Assert.Contains(booksPage, b => b.Id == book1.Id || b.Id == book2.Id);
        }

        [Fact]
        public async Task GetBookByISBN_Success()
        {
            // Arrange
            var book = BookTestData.CreateTestBook();
            await Context.Books.AddAsync(book);
            await Context.SaveChangesAsync();
            var bookRepository = new BookRepository(Context, _mockWebHostEnvironment.Object, _mockConfiguration.Object);

            // Act
            var result = await bookRepository.GetByISBN(book.ISBN, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.ISBN, result!.ISBN);
        }
    }
}
