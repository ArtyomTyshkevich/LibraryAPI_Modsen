using Library.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

public class ImageCacheService : IImageCacheService
{
    private readonly IDistributedCache _cache;
    private readonly string _imageFolderPath;
    public ImageCacheService(IConfiguration configuration, IDistributedCache cache)
    {
        _imageFolderPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, configuration["ImageStorage:Path"]!);
        _cache = cache;
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
    public async Task BookDTOCreate(BookDTO bookDTO)
    {
        if (bookDTO.ImageFileName == null)
        {
            return;
        }

        // Загружаем изображение с помощью асинхронного метода
        var imageBytes = await GetImageAsync(bookDTO.ImageFileName);

        if (imageBytes.Length > 0)
        {
            // Преобразуем байты изображения в IFormFile
            bookDTO.ImageFile = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, bookDTO.ImageFileName, bookDTO.ImageFileName);
        }

        return;
    }
}