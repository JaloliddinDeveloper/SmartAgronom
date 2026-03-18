using AqlliAgronom.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace AqlliAgronom.Infrastructure.Services;

public class LocalFileStorageService(
    ITelegramBotClient botClient,
    IOptions<FileStorageOptions> options,
    ILogger<LocalFileStorageService> logger) : IFileStorageService
{
    private readonly FileStorageOptions _opts = options.Value;

    public async Task<string> SaveTelegramPhotoAsync(string telegramFileId, CancellationToken ct = default)
    {
        // Ensure target directory exists
        var productDir = Path.Combine(_opts.BasePath, "products");
        Directory.CreateDirectory(productDir);

        // Get file info from Telegram
        var file = await botClient.GetFile(telegramFileId, ct);

        // Determine extension from Telegram FilePath (e.g. "photos/file_xxx.jpg")
        var ext = Path.GetExtension(file.FilePath ?? ".jpg");
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var fileName  = $"{Guid.NewGuid()}{ext}";
        var localPath = Path.Combine(productDir, fileName);

        // Download photo bytes and write to disk
        await using (var fileStream = File.Create(localPath))
        {
            await botClient.DownloadFile(file.FilePath!, fileStream, ct);
        }

        var publicUrl = $"{_opts.BaseUrl.TrimEnd('/')}/products/{fileName}";
        logger.LogInformation("Saved Telegram photo {FileId} → {Path} (URL: {Url})",
            telegramFileId, localPath, publicUrl);

        return publicUrl;
    }
}
