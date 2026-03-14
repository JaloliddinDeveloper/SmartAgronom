namespace AqlliAgronom.Infrastructure.AI.Embedding;

public class EmbeddingOptions
{
    public const string SectionName = "Embedding";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.voyageai.com";
    public string ModelId { get; set; } = "voyage-3";
    public int VectorDimension { get; set; } = 1024;
}
