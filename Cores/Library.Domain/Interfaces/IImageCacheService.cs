using Library.Domain.DTOs;
public interface IImageCacheService
{
    /// <summary>
    /// Получает изображение из кэша или загружает его из источника.
    /// </summary>
    /// <param name="imageKey">Ключ изображения для кэша.</param>
    /// <returns>Массив байтов изображения.</returns>
    Task<byte[]> GetImageAsync(string imageKey);

    /// <summary>
    /// Загружает изображение из источника (файловой системы).
    /// </summary>
    /// <param name="imageFileName">Имя файла изображения.</param>
    /// <returns>Массив байтов изображения.</returns>
    Task<byte[]> LoadImageFromSourceAsync(string imageFileName);
    Task BookDTOCreate(BookDTO bookDTO);
}
