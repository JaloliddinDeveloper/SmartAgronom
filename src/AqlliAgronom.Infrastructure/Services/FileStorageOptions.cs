namespace AqlliAgronom.Infrastructure.Services;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>Absolute path on disk where uploads are stored (e.g. /app/uploads).</summary>
    public string BasePath { get; set; } = "/app/uploads";

    /// <summary>Public base URL used to build the returned file URL (e.g. https://domain.com/uploads).</summary>
    public string BaseUrl { get; set; } = "http://localhost:8080/uploads";
}
