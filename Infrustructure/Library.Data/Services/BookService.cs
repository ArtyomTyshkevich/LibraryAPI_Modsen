using AutoMapper;
using Library.Application.DTOs;
using Library.Application.Exceptions;
using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace Library.Data.Services
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageCacheService _imageCacheService;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IWebHostEnvironment _env;

        public BookService(LibraryDbContext libraryDbContext, IMapper mapper, IUnitOfWork unitOfWork, IImageCacheService imageCacheService, IConfiguration configuration, IPublishEndpoint publishEndpoint, IWebHostEnvironment env)
        {
            _env = env;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageCacheService = imageCacheService;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
        }

        public async Task AddImageToBook(BookDTO bookDTO, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.Books.Get(bookDTO.Id, cancellationToken);
            if (book == null)
            {
                throw new BookNotFoundException(bookDTO.Id); // Исключение, если книга не найдена
            }

            if (bookDTO.ImageFile == null)
            {
                throw new ArgumentException("Image file is missing.");
            }

            await DeleteOldImageIfExists(book!, cancellationToken);
            var fileName = await SaveNewImage(bookDTO, cancellationToken);

            book.ImageFileName = fileName;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private Task DeleteOldImageIfExists(Book book, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(book.ImageFileName)) return Task.CompletedTask;

            string oldImagePath = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!, book.ImageFileName!);
            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }

            return Task.CompletedTask;
        }

        private async Task<string> SaveNewImage(BookDTO bookDTO, CancellationToken cancellationToken)
        {
            string uploadFolder = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!);
            var fileName = Guid.NewGuid().ToString() + "_" + bookDTO.ImageFile!.FileName;
            string filepath = Path.Combine(uploadFolder, fileName);

            using (var stream = File.Create(filepath))
            {
                await bookDTO.ImageFile.CopyToAsync(stream, cancellationToken);
            }
            return fileName;
        }

        public async Task<string> AddBookToUser(Guid bookId, long userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.Get(userId, cancellationToken);
            var book = await _unitOfWork.Books.Get(bookId, cancellationToken);

            if (book == null)
            {
                throw new BookNotFoundException(bookId); // Исключение, если книга не найдена
            }

            if (book.StartRentDateTime != null && book.EndRentDateTime == null)
            {
                throw new BookAlreadyRentedException(bookId); // Исключение, если книга уже выдана
            }

            book.StartRentDateTime = DateTime.UtcNow;
            book.EndRentDateTime = DateTime.UtcNow.AddDays(60);
            user!.Books.Add(book);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "The book was issued successfully";
        }

        public async Task<BookDTO> BooksByIdRedis(Guid id, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.Books.Get(id, cancellationToken);
            if (book == null)
            {
                throw new BookNotFoundException(id); // Исключение, если книга не найдена
            }

            var bookDTO = _mapper.Map<BookDTO>(book);
            var formFile = await _imageCacheService.BookDTOCreateWithRedis(bookDTO, cancellationToken);
            bookDTO.ImageFile = formFile;
            return bookDTO;
        }

        public async Task<Book?> BookToUser(Guid bookId, long userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.Get(userId, cancellationToken);
            var book = await _unitOfWork.Books.Get(bookId, cancellationToken);

            if (book == null)
            {
                throw new BookNotFoundException(bookId); // Исключение, если книга не найдена
            }

            if (book.StartRentDateTime != null && book.EndRentDateTime == null)
            {
                throw new BookAlreadyRentedException(bookId); // Исключение, если книга уже выдана
            }

            book.StartRentDateTime = DateTime.UtcNow;
            book.EndRentDateTime = DateTime.UtcNow.AddDays(60);
            user!.Books.Add(book);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _publishEndpoint.Publish(book);

            return book;
        }

        public async Task<BookDTO> BooksByISBNFileSystem(string ISBN, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.Books.GetByISBN(ISBN, cancellationToken);
            if (book == null)
            {
                throw new BookNotFoundException(new Guid());
            }

            var bookDTO = _mapper.Map<BookDTO>(book);

            if (bookDTO.ImageFileName != null)
            {
                bookDTO.ImageFile = await GetImageFileFromPath(book.ImageFileName!, cancellationToken);
            }

            return bookDTO;
        }

        private async Task<IFormFile?> GetImageFileFromPath(string fileName, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!, fileName);
            if (File.Exists(filePath))
            {
                var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
                return new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, fileName, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpg"
                };
            }

            return null;
        }
    }
}