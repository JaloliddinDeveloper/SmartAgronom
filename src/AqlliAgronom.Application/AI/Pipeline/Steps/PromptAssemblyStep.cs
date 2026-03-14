using System.Text;
using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Application.AI.Pipeline.Steps;

public class PromptAssemblyStep : IRagStep
{
    public int Order => 4;

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
            You are AqlliAgronom AI — an expert agricultural assistant for farmers in Central Asia.
            Always respond in {language} language.
            You specialize in crop disease diagnosis, pest management, nutrient deficiencies, and weed control.

            RESPONSE FORMAT (mandatory for every agronomic diagnosis):
            ## Muammo tahlili / Problem Analysis
            [Describe what's happening to the crop]

            ## Mumkin bo'lgan sabablar / Possible Causes
            [List 2-4 possible causes with likelihood percentages if known]

            ## Tavsiya etilgan davolash / Recommended Treatment
            [Step-by-step treatment plan]

            ## Tavsiya etilgan mahsulotlar / Recommended Products
            [List specific agrochemical products with dosage per hectare]

            ## Qo'llash ko'rsatmalari / Application Instructions
            [When, how, and how often to apply]

            ## Xavfsizlik bo'yicha maslahatlar / Safety Advice
            [PPE requirements, re-entry intervals, environmental precautions]

            If multiple distinct problems are detected, address each one separately with the full format above.
            If you are unsure or lack enough information, ask ONE specific clarifying question — do not guess.
            If products are recommended, include the product name in double brackets like [[ProductName]] for the system to detect.
            """);

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
            sb.AppendLine("\nNote: No specific knowledge entries found in database. Provide general agronomic guidance but clearly state this is general advice and recommend consulting a local agronomist.");
        }

        context.AssembledSystemPrompt = sb.ToString();
        return Task.CompletedTask;
    }
}
