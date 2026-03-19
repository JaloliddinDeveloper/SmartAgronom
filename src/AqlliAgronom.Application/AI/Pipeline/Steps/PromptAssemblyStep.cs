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
            You are AqlliAgronom AI — a shop assistant for an agricultural store.
            Always respond in {language} language.

            YOUR ONLY JOB — follow these two rules, nothing else:

            RULE 1 — If a product from the AVAILABLE PRODUCTS list matches the farmer's problem or question:
            Write ONE short sentence (max 10 words) saying this product helps, then write [[ProductName]].
            Example: "Shiralarga qarshi [[Aktara 25 WG]] yordam beradi."
            That's it. Nothing more.

            RULE 2 — If NO product in the AVAILABLE PRODUCTS list matches:
            Write EXACTLY this one sentence, nothing more, nothing less:
            "Kechirasiz, bizning bazada bu muammoga yechim bo'ladigan mahsulot yo'q ekan, bizning agronom bilan bog'lanib ko'ring, u sizga yordam bera oladi degan umiddaman Tel: 909660361"

            FORBIDDEN:
            - Do NOT explain what the product does.
            - Do NOT give agronomic advice, treatment steps, or tips.
            - Do NOT compare products.
            - Do NOT mention products not in the AVAILABLE PRODUCTS list.
            - Do NOT write more than 2 sentences total.
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
            sb.AppendLine("\nAVAILABLE PRODUCTS: (none — the store has no products yet)");
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
        // No knowledge chunks found — Claude still has the product list to work with


        context.AssembledSystemPrompt = sb.ToString();
        return Task.CompletedTask;
    }
}
