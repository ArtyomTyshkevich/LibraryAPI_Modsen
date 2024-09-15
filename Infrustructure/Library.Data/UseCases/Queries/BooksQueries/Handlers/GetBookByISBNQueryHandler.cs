using AutoMapper;
using Library.Application.DTOs;
using Library.Application.Exceptions;
using Library.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Library.Data.UseCases.Queries.BooksQueries;

namespace Library.Data.UseCases.Queries.BooksQueries.Handlers
{
    public class GetBookByISBNQueryHandler : IRequestHandler<GetBookByISBNQuery, BookDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public GetBookByISBNQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _env = env;
        }

        public async Task<BookDTO> Handle(GetBookByISBNQuery request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.Books.GetByISBN(request.ISBN, cancellationToken);
            if (book == null)
            {
                throw new BookNotFoundException(new Guid()); // Можно заменить на ISBN, если это более информативно
            }

            var bookDTO = _mapper.Map<BookDTO>(book);

            if (bookDTO.ImageFileName != null)
            {
                bookDTO.ImageFile = await GetImageFileFromPath(bookDTO.ImageFileName!, cancellationToken);
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
