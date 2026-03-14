namespace AqlliAgronom.Infrastructure.AI.Claude;

public class ClaudeOptions
{
    public const string SectionName = "Claude";

    public string ApiKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = "claude-opus-4-6";
    public int MaxTokens { get; set; } = 4096;
    public double Temperature { get; set; } = 0.3;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
}
