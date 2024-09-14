using AutoMapper;
using Library.Application.DTOs;
using Library.Application.Exceptions;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


namespace Library.Data.UseCases.Commands.Handlers
{
    public class AddImageToBookCommandHandler : IRequestHandler<AddImageToBookCommand>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageCacheService _imageCacheService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AddImageToBookCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IImageCacheService imageCacheService, IConfiguration configuration, IWebHostEnvironment env)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageCacheService = imageCacheService;
            _configuration = configuration;
            _env = env;
        }

        public async Task<Unit> Handle(AddImageToBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.Books.Get(request.BookDTO.Id, cancellationToken);
            if (book == null)
            {
                throw new BookNotFoundException(request.BookDTO.Id);
            }

            if (request.BookDTO.ImageFile == null)
            {
                throw new ArgumentException("Image file is missing.");
            }

            await DeleteOldImageIfExists(book, cancellationToken);
            var fileName = await SaveNewImage(request.BookDTO, cancellationToken);

            book.ImageFileName = fileName;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
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
    }

}
