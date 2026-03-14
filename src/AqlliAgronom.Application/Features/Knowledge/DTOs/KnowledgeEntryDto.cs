namespace AqlliAgronom.Application.Features.Knowledge.DTOs;

public record KnowledgeEntryDto(
    Guid Id,
    string Title,
    string CropName,
    string ProblemName,
    string Category,
    string Symptoms,
    string DetailedExplanation,
    string RecommendedProducts,
    string DosagePerHectare,
    string ApplicationInstructions,
    string SafetyPrecautions,
    string Language,
    string? GrowthStage,
    string? RegionConsiderations,
    string[] Tags,
    string Status,
    int Version,
    Guid? VectorId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record KnowledgeEntrySummaryDto(
    Guid Id,
    string Title,
    string CropName,
    string ProblemName,
    string Category,
    string Status,
    bool IsIndexed,
    DateTime CreatedAt);
