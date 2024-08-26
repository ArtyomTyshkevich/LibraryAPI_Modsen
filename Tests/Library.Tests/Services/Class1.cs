using AutoMapper;
using Library.Data.Context;
using Library.Data.Services;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Tests.Common;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests.Services
{
    public class BookServiceTests : IAsyncLifetime
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IImageCacheService> _imageCacheServiceMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private LibraryDbContext _context;
        private BookService _service;
        private string _testUploadPath;

        public BookServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _imageCacheServiceMock = new Mock<IImageCacheService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _envMock = new Mock<IWebHostEnvironment>();
            _configurationMock = new Mock<IConfiguration>();
            _testUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Tests", "ImageFile");
            Directory.CreateDirectory(_testUploadPath);
            _envMock.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _configurationMock.Setup(c => c["ImageStorage:Path"]).Returns("Tests/ImageFile");
        }
        public async Task InitializeAsync()
        {
            _context = LibraryContextFactory.CreateInMemory();
            _service = new BookService(
                _context,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _imageCacheServiceMock.Object,
                _configurationMock.Object,
                _publishEndpointMock.Object,
                _envMock.Object
            );
        }

        public Task DisposeAsync()
        {
            LibraryContextFactory.Destroy(_context);
            return Task.CompletedTask;
        }

        [Fact]
        public async Task BooksPagination_Success()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Name = "Book 1" },
                new Book { Id = Guid.NewGuid(), Name = "Book 2" },
                new Book { Id = Guid.NewGuid(), Name = "Book 3" }
            };
            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.BooksPagination(1, 2);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task BooksByIdRedis_Success()
        {
            // Arrange
            var bookId = LibraryContextFactory.BookIdForUpdate; // Use an existing book ID from the factory
            var book = await _context.Books.FindAsync(bookId);
            var bookDTO = new BookDTO { Id = bookId };

            _unitOfWorkMock.Setup(u => u.Books.Get(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDTO>(book)).Returns(bookDTO);
            _imageCacheServiceMock.Setup(i => i.BookDTOCreate(bookDTO)).ReturnsAsync(new FormFile(new MemoryStream(), 0, 0, "Data", "image.jpg"));

            // Act
            var result = await _service.BooksByIdRedis(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
        }

        [Fact]
        public async Task BookToUser_Success()
        {
            // Arrange
            var bookId = LibraryContextFactory.BookIdForUpdate; // Use an existing book ID from the factory
            var userId = 1L;

            var user = await _context.Users.FindAsync(userId);
            var book = await _context.Books.FindAsync(bookId);

            _unitOfWorkMock.Setup(u => u.Books.Get(bookId)).ReturnsAsync(book);

            // Act
            var result = await _service.BookToUser(bookId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
        }

        [Fact]
        public async Task BooksByISBNFileSystem_Success()
        {
            // Arrange
            var isbn = "2222222222"; // Use an existing ISBN from the factory
            var book = await _context.Books.FirstAsync(b => b.ISBN == isbn);
            var bookDTO = new BookDTO { Id = book.Id, ISBN = isbn, ImageFileName = "image.jpg" };

            _mapperMock.Setup(m => m.Map<BookDTO>(book)).Returns(bookDTO);
            _envMock.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _configurationMock.Setup(c => c["ImageStorage:Path"]).Returns("images");

            // Act
            var result = await _service.BooksByISBNFileSystem(isbn);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(isbn, result.ISBN);
        }
    }
}