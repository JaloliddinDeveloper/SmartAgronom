using System.Text;
using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Application.AI.Pipeline.Steps;

public class PromptAssemblyStep : IRagStep
{
    public int Order => 5;

    public Task ExecuteAsync(RagPipelineContext context, CancellationToken ct)
    {
        var language = context.UserLanguage switch
        {
            Language.Uzbek => "o'zbek",
            Language.Russian => "russian",
            _ => "english"
        };

        var sb = new StringBuilder();

        sb.AppendLine($"""
            You are AqlliAgronom AI — a practical agricultural assistant for farmers in Central Asia.
            Always respond in {language} language.

            RULES (follow strictly):
            1. Be CONCISE. Give short, practical answers — 3 to 8 sentences maximum.
            2. Diagnose the farmer's problem briefly (1-2 sentences).
            3. Recommend ONLY products that appear in the KNOWLEDGE BASE section below. Do NOT invent or suggest products not in the knowledge base.
            4. If you recommend a product, write its name in double brackets: [[ProductName]]. Only use the exact product names from the knowledge base.
            5. If the knowledge base has no matching products, say so clearly — do not suggest generic product names.
            6. If you need more information, ask ONE short clarifying question.
            7. Do NOT use large tables, long lists, or section headers. Write in plain, friendly sentences a farmer can understand.
            """);

        // Inject the exact list of products available in the store
        if (context.AvailableProducts.Count > 0)
        {
            sb.AppendLine("\n--- AVAILABLE PRODUCTS IN OUR STORE (you may ONLY recommend these) ---");
            foreach (var name in context.AvailableProducts)
                sb.AppendLine($"- {name}");
            sb.AppendLine("--- END OF PRODUCT LIST ---");
        }
        else
        {
            sb.AppendLine("\nNote: There are currently no products in our store. Do not recommend any products.");
        }

        // Inject retrieved knowledge context
        if (context.RankedChunks.Count > 0)
        {
            sb.AppendLine("\n--- AGRONOMIC KNOWLEDGE BASE (use this as primary source) ---");
            for (int i = 0; i < context.RankedChunks.Count; i++)
            {
                var chunk = context.RankedChunks[i];
                sb.AppendLine($"\n[Knowledge {i + 1}] Crop: {chunk.CropName} | Problem: {chunk.ProblemName} | Relevance: {chunk.Score:P0}");
                sb.AppendLine(chunk.Content);
                sb.AppendLine("---");
            }
        }
        else
        {
            sb.AppendLine("\nNote: No knowledge entries found in the database for this query. Give a brief general answer and tell the farmer you have no specific products to recommend — suggest they contact a local agronomist.");
        }

        context.AssembledSystemPrompt = sb.ToString();
        return Task.CompletedTask;
    }
}
