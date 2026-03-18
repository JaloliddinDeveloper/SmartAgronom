namespace AqlliAgronom.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>Downloads a Telegram photo by fileId and saves it to disk. Returns the public URL.</summary>
    Task<string> SaveTelegramPhotoAsync(string telegramFileId, CancellationToken ct = default);
}
