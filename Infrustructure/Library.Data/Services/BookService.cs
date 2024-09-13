﻿using AutoMapper;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Data.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageCacheService _imageCacheService;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IWebHostEnvironment _env;

        public BookService(LibraryDbContext libraryDbContext, IMapper mapper, IUnitOfWork unitOfWork, IImageCacheService imageCacheService, IConfiguration configuration, IPublishEndpoint publishEndpoint, IWebHostEnvironment env)
        {
            _env = env;
            _libraryDbContext = libraryDbContext;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageCacheService = imageCacheService;
            _configuration = configuration;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
        }

        public async Task AddImageToBook(BookDTO BookDTO)
        {
            Book? book = await _unitOfWork.Books.Get(BookDTO.Id);
            if (BookDTO.ImageFile == null)
            {
                return;
            }
            if (book!.ImageFileName != null)
            {
                string OldImagePath = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!, book.ImageFileName);
                System.IO.File.Delete(OldImagePath);
            }
            string uploadFolder = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!);
            var fileName = Guid.NewGuid().ToString() + "_" + BookDTO.ImageFile.FileName;
            string filepath = Path.Combine(uploadFolder, fileName);
            using (var stream = System.IO.File.Create(filepath))
            {
                BookDTO.ImageFile.CopyTo(stream);
            }
            book.ImageFileName = fileName;
            await _libraryDbContext.SaveChangesAsync();
            return;
        }
        public async Task<string> AddBookToUser(Guid bookId, long userId)
        {
            var user = await _libraryDbContext.Users
                                 .Include(u => u.Books)
                                 .FirstOrDefaultAsync(x => x.Id == userId);
            var book = await _unitOfWork.Books.Get(bookId);
            if (book == null)
            {
                return "Book not found";
            }
            if (book.StartRentDateTime != null && book.EndRentDateTime == null)
            {
                return "The book has already been issued to another user";
            }
            book.StartRentDateTime = DateTime.UtcNow;
            book.EndRentDateTime = DateTime.UtcNow.AddDays(60);
            user!.Books.Add(book);

            await _libraryDbContext.SaveChangesAsync();

            return "The book was issued successfully";

        }
        public async Task<List<Book>> BooksPagination(int pageNum, int pageSize)
        {
            var books = await _libraryDbContext.Books
                                 .Skip((pageNum - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
            return books;
        }
        public async Task<BookDTO> BooksByIdRedis(Guid id)
        {
            var book = await _unitOfWork.Books.Get(id);
            var bookDTO = _mapper.Map<BookDTO>(book);
            var formFile = await _imageCacheService.BookDTOCreate(bookDTO);
            bookDTO.ImageFile = formFile;
            return bookDTO;
        }
        public async Task<Book?> BookToUser(Guid bookId, long userId)
        {
            var user = await _libraryDbContext.Users
                      .Include(u => u.Books)
                      .FirstOrDefaultAsync(x => x.Id == userId);
            var book = await _unitOfWork.Books.Get(bookId);

            if (book!.StartRentDateTime != null && book.EndRentDateTime == null)
            {
                return null;
            }
            book.StartRentDateTime = DateTime.UtcNow;
            book.EndRentDateTime = DateTime.UtcNow.AddDays(60);

            user!.Books.Add(book);
            await _publishEndpoint.Publish(book);

            await _libraryDbContext.SaveChangesAsync();

            return book;
        }
        public async Task<BookDTO> BooksByISBNFileSystem(string ISBN)
        {
            var book = await _libraryDbContext.Books
                                          .Include(b => b.Author)
                                          .FirstAsync(a => a.ISBN == ISBN);
            var bookDTO = _mapper.Map<BookDTO>(book);
            if (bookDTO.ImageFileName != null)
            {
                var filePath = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!, book.ImageFileName);

                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    var file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, book.ImageFileName, book.ImageFileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpg"
                    };
                    bookDTO.ImageFile = file;
                }
            }

                return bookDTO;
        }
    }
}
