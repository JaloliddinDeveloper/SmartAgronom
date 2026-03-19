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
            You are AqlliAgronom AI — a product recommendation assistant for an agricultural store.
            Always respond in {language} language.

            YOUR ONLY JOB:
            - Look at the farmer's problem.
            - Check the AVAILABLE PRODUCTS list below.
            - If one or more products match the farmer's problem: briefly diagnose (1 sentence), then recommend the matching product(s) using [[ProductName]] syntax.
            - If NO products in the list match the farmer's problem: respond with EXACTLY this (translated to {language}):
              "Kechirasiz, bizning bazada bu muammoga yechim bo'ladigan mahsulot yo'q ekan, bizning agronom bilan bog'lanib ko'ring, u sizga yordam bera oladi degan umiddaman Tel: 909660361"
              Do NOT add any other text, tips, or advice when no products match.

            STRICT RULES:
            - NEVER recommend products not in the AVAILABLE PRODUCTS list.
            - NEVER give general agronomic advice if no matching product exists.
            - Do NOT use tables, section headers, or long lists.
            - Write in plain, friendly sentences.
            - If you need more info, ask ONE short question.
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
