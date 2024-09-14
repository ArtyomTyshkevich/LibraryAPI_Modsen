using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Library.Data.UseCases.Queries.Handlers
{
    public class GetImageQueryHandler : IRequestHandler<GetImageQuery, byte[]>
    {
        private readonly IDistributedCache _cache;
        private readonly string _imageFolderPath;

        public GetImageQueryHandler(IDistributedCache cache, string imageFolderPath)
        {
            _cache = cache;
            _imageFolderPath = imageFolderPath;
        }

        public async Task<byte[]> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {
            var imageBytes = await _cache.GetAsync(request.ImageKey, cancellationToken);
            if (imageBytes != null)
            {
                return imageBytes;
            }

            imageBytes = await LoadImageFromSource(request.ImageKey, cancellationToken);
            await CacheImageAsync(request.ImageKey, imageBytes, cancellationToken);

            return imageBytes;
        }

        private async Task<byte[]> LoadImageFromSource(string imageFileName, CancellationToken cancellationToken)
        {
            var filePath = GetFilePath(imageFileName);

            if (!File.Exists(filePath))
            {
                return new byte[0];
            }

            return await ReadImageFile(filePath, cancellationToken);
        }

        private string GetFilePath(string imageFileName)
        {
            return Path.Combine(_imageFolderPath, imageFileName);
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
    }
}