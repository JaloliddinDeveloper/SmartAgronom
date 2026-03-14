using System.Text.RegularExpressions;

namespace AqlliAgronom.Application.AI.Pipeline.Steps;

public class QueryPreprocessingStep : IRagStep
{
    public int Order => 1;

    private static readonly Dictionary<string, string> CropAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["tomato"] = "pomidor", ["tomat"] = "pomidor", ["помидор"] = "pomidor",
        ["cotton"] = "paxta", ["хлопок"] = "paxta",
        ["wheat"] = "bug'doy", ["пшеница"] = "bug'doy",
        ["potato"] = "kartoshka", ["картошка"] = "kartoshka", ["картофель"] = "kartoshka",
        ["onion"] = "piyoz", ["лук"] = "piyoz",
        ["corn"] = "makkajo'xori", ["maize"] = "makkajo'xori", ["кукуруза"] = "makkajo'xori",
    };

    private static readonly string[] SymptomKeywords =
    [
        "yellow", "sarg'", "sariq", "жёлт",
        "spot", "dog'", "пятно",
        "curl", "qayril", "скручива",
        "wilt", "so'l", "увяда",
        "rot", "chirigan", "гниль",
        "hole", "teshik", "дыра",
        "black", "qora", "чёрн",
        "brown", "jigarrang", "коричнев",
        "white", "oq", "бел",
        "mold", "mog'or", "плесень",
        "insect", "hasharot", "насекомое",
        "larva", "qurt", "личинка",
        "stunted", "o'smayapti", "отстаёт в росте",
    ];

    public Task ExecuteAsync(RagPipelineContext context, CancellationToken ct)
    {
        var query = context.UserQuery.Trim();

        // Basic normalization
        query = Regex.Replace(query, @"\s+", " ");

        // Detect crop name
        foreach (var (alias, canonical) in CropAliases)
        {
            if (query.Contains(alias, StringComparison.OrdinalIgnoreCase))
            {
                context.DetectedCrop = canonical;
                break;
            }
        }

        // Extract symptoms mentioned
        var symptoms = SymptomKeywords
            .Where(kw => query.Contains(kw, StringComparison.OrdinalIgnoreCase))
            .ToList();
        context.DetectedSymptoms = symptoms;

        // Build enriched query for embedding
        var enriched = context.DetectedCrop is not null
            ? $"{context.DetectedCrop}: {query}"
            : query;

        context.NormalizedQuery = enriched;

        return Task.CompletedTask;
    }
}
