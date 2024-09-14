using Library.Application.DTOs;
using Microsoft.AspNetCore.Http;


namespace Library.Application.Interfaces
{
    public interface IImageCacheService
    {
        /// <summary>
        /// Получает изображение из кэша или загружает его из источника.
        /// </summary>
        /// <param name="imageKey">Ключ изображения для кэша.</param>
        /// <returns>Массив байтов изображения.</returns>
        Task<byte[]> GetImage(string imageKey, CancellationToken cancellationToken = default);


        Task<IFormFile> BookDTOCreateWithRedis(BookDTO bookDTO, CancellationToken cancellationToken = default);
    }
}
