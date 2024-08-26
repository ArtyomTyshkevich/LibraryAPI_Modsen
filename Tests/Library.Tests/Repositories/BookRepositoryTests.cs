using Library.Data.Repositories;
using Library.Domain.Entities;
using Library.Tests.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests.Books
{
    public class BookRepositoryTests : TestCommandBase
    {
        [Fact]
        public async Task CreateBook_Success()
        {
            // Arrange
            var bookId = Guid.Parse("99999999-9999-9999-9999-999999999999");
            var book = new Book
            {
                Id = bookId,
                Name = "Test Book",
                ISBN = "9876543210",
                Description = "Test Description",
                StartRentDateTime = DateTime.Today.AddDays(-3),
                EndRentDateTime = DateTime.Today.AddDays(-1),
                ImageFileName = "testimage.jpg"
            };
            var bookRepository = new BookRepository(Context, null!, null!);

            // Act
            await bookRepository.Create(book);
            await Context.SaveChangesAsync();

            // Assert
            var createdBook = await Context.Books.FirstOrDefaultAsync(b =>
                b.Id == book.Id &&
                b.Name == book.Name &&
                b.ISBN == book.ISBN &&
                b.Description == book.Description &&
                b.StartRentDateTime == book.StartRentDateTime &&
                b.EndRentDateTime == book.EndRentDateTime &&
                b.ImageFileName == book.ImageFileName);

            Assert.NotNull(createdBook);
        }

        [Fact]
        public async Task GetBookById_Success()
        {
            // Arrange
            var bookId = Guid.Parse("88888888-8888-8888-8888-888888888889");
            var book = new Book
            {
                Id = bookId,
                Name = "Test Book",
                ISBN = "1234567890",
                Description = "Test Description",
                StartRentDateTime = DateTime.Today.AddDays(-10),
                EndRentDateTime = DateTime.Today.AddDays(-5),
            };
            await Context.Books.AddAsync(book);
            await Context.SaveChangesAsync();
            var bookRepository = new BookRepository(Context, null!, null!);

            // Act
            var result = await bookRepository.Get(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
            Assert.Equal("Test Book", result.Name);
            Assert.Equal("1234567890", result.ISBN);
            Assert.Equal("Test Description", result.Description);
            Assert.Equal(DateTime.Today.AddDays(-10), result.StartRentDateTime);
            Assert.Equal(DateTime.Today.AddDays(-5), result.EndRentDateTime);
        }

        [Fact]
        public async Task DeleteBookById_Success()
        {
            // Arrange
            var bookId = Guid.Parse("77777777-7777-7777-7777-777777777779");
            var book = new Book
            {
                Id = bookId,
                Name = "Test Book",
                ISBN = "1111111111",
                Description = "Test Description",
                StartRentDateTime = DateTime.Today.AddDays(-15),
                EndRentDateTime = DateTime.Today.AddDays(-10),
            };
            await Context.Books.AddAsync(book);
            await Context.SaveChangesAsync();
            var bookRepository = new BookRepository(Context, null!, null!);

            // Act
            await bookRepository.Delete(book);
            await Context.SaveChangesAsync();

            // Assert
            var result = await Context.Books.FindAsync(bookId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateBook_Success()
        {
            // Arrange
            var book = await Context.Books.FindAsync(LibraryContextFactory.BookIdForUpdate);
            book!.Name = "Updated Book";
            book.ISBN = "2222222222";
            book.Description = "Updated Description";
            book.StartRentDateTime = DateTime.Today.AddDays(-8);
            book.EndRentDateTime = DateTime.Today.AddDays(-3);
            book.ImageFileName = "updatedimage.jpg";
            var bookRepository = new BookRepository(Context, null!, null!);

            // Act
            await bookRepository.Update(book);
            await Context.SaveChangesAsync();

            // Assert
            var updatedBook = await Context.Books.FindAsync(LibraryContextFactory.BookIdForUpdate);
            Assert.NotNull(updatedBook);
            Assert.Equal("Updated Book", updatedBook.Name);
            Assert.Equal("2222222222", updatedBook.ISBN);
            Assert.Equal("Updated Description", updatedBook.Description);
            Assert.Equal(DateTime.Today.AddDays(-8), updatedBook.StartRentDateTime);
            Assert.Equal(DateTime.Today.AddDays(-3), updatedBook.EndRentDateTime);
            Assert.Equal("updatedimage.jpg", updatedBook.ImageFileName);
        }
    }
}