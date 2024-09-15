using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace Library.Data.UseCases.Commands.BooksCommands.Handlers
{
    public class CreateFormFileFromBookDTOCommandHandler : IRequestHandler<CreateFormFileFromBookDTOCommand, IFormFile?>
    {
        private readonly IDistributedCache _cache;
        private readonly string _imageFolderPath;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public CreateFormFileFromBookDTOCommandHandler(IConfiguration configuration, IDistributedCache cache, IWebHostEnvironment env)
        {
            _env = env;
            _cache = cache;
            _configuration = configuration;
            _imageFolderPath = Path.Combine(_env.ContentRootPath, _configuration["ImageStorage:Path"]!);
        }

        public async Task<IFormFile?> Handle(CreateFormFileFromBookDTOCommand request, CancellationToken cancellationToken)
        {
            var bookDTO = request.BookDTO;
            if (bookDTO.ImageFileName == null)
            {
                return null;
            }

            var imageBytes = await GetImage(bookDTO.ImageFileName, cancellationToken);

            if (imageBytes.Length == 0)
            {
                return null;
            }

            return CreateFormFile(bookDTO.ImageFileName, imageBytes);
        }

        private async Task<byte[]> GetImage(string imageKey, CancellationToken cancellationToken)
        {
            var imageBytes = await _cache.GetAsync(imageKey, cancellationToken);
            if (imageBytes != null)
            {
                return imageBytes;
            }

            imageBytes = await LoadImageFromSource(imageKey, cancellationToken);
            await CacheImageAsync(imageKey, imageBytes, cancellationToken);

            return imageBytes;
        }

        private async Task<byte[]> LoadImageFromSource(string imageFileName, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_imageFolderPath, imageFileName);

            if (!File.Exists(filePath))
            {
                return new byte[0];
            }

            return await ReadImageFile(filePath, cancellationToken);
        }

        private async Task<byte[]> ReadImageFile(string filePath, CancellationToken cancellationToken)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
        }

        private async Task CacheImageAsync(string imageKey, byte[] imageBytes, CancellationToken cancellationToken)
        {
            await _cache.SetAsync(imageKey, imageBytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            }, cancellationToken);
        }

        private IFormFile CreateFormFile(string fileName, byte[] imageBytes)
        {
            var contentType = "image/jpg";
            return new FormFile(
                new MemoryStream(imageBytes),
                0,
                imageBytes.Length,
                fileName,
                fileName
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}
