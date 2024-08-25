using Library.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace Library.Data.Services
{
    public class ImageCacheService : IImageCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly string _imageFolderPath;
        private readonly IConfiguration _configuration;

        public ImageCacheService(IConfiguration configuration, IDistributedCache cache)
        {
            _cache = cache;
            _configuration = configuration;
            _imageFolderPath = _configuration.GetValue<string>("PathToImages");
        }

        public async Task<byte[]> GetImageAsync(string imageKey)
        {
            var imageBytes = await _cache.GetAsync(imageKey);
            if (imageBytes != null)
            {
                return imageBytes;
            }

            imageBytes = await LoadImageFromSourceAsync(imageKey);

            await _cache.SetAsync(imageKey, imageBytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });

            return imageBytes;
        }

        public async Task<byte[]> LoadImageFromSourceAsync(string imageFileName)
        {
            var filePath = Path.Combine(_imageFolderPath, imageFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return new byte[0];
            }

            byte[] imageBytes;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
            }

            return imageBytes;
        }
        public async Task<IFormFile?> BookDTOCreate(BookDTO bookDTO)
        {
            if (bookDTO.ImageFileName == null)
            {
                return null; // Если имя файла не указано, возвращаем null
            }

            // Получаем байты изображения асинхронно
            var imageBytes = await GetImageAsync(bookDTO.ImageFileName);

            if (imageBytes.Length == 0)
            {
                return null; // Если изображение пустое, возвращаем null
            }

            // Указываем тип содержимого (например, "image/jpeg")
            var contentType = "image/jpg"; // Установите это значение в зависимости от типа изображения или определите его динамически

            // Создаем объект FormFile с нужными параметрами
            var formFile = new FormFile(
                new MemoryStream(imageBytes),
                0,
                imageBytes.Length,
                bookDTO.ImageFileName,
                bookDTO.ImageFileName
            )
            {
                Headers = new HeaderDictionary(), // Инициализируем заголовки
                ContentType = contentType          // Устанавливаем тип содержимого
            };

            return formFile;
        }
    } 
}