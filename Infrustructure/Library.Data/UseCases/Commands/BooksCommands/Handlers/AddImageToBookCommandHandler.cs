using Library.Application.Exceptions;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Library.Data.UseCases.Commands.BooksCommands;

namespace Library.Data.UseCases.Commands.BooksCommands.Handlers
{
    public class AddImageToBookCommandHandler : IRequestHandler<AddImageToBookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public AddImageToBookCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment env, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _configuration = configuration;
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
            var fileName = await SaveNewImage(request.BookDTO.ImageFile, cancellationToken);

            book.ImageFileName = fileName;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value; // Ensure you return Unit.Value
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

        private async Task<string> SaveNewImage(IFormFile imageFile, CancellationToken cancellationToken)
        {
            string uploadFolder = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!);
            var fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filepath = Path.Combine(uploadFolder, fileName);

            using (var stream = File.Create(filepath))
            {
                await imageFile.CopyToAsync(stream, cancellationToken);
            }
            return fileName;
        }
    }
}