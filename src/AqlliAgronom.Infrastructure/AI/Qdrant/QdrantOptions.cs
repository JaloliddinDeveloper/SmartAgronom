namespace AqlliAgronom.Infrastructure.AI.Qdrant;

public class QdrantOptions
{
    public const string SectionName = "Qdrant";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6334;
    public string? ApiKey { get; set; }
    public string KnowledgeCollectionName { get; set; } = "agronomic_knowledge";
    public int VectorDimension { get; set; } = 1024;
}
