using AqlliAgronom.Application.Features.Knowledge.DTOs;
using AqlliAgronom.Domain.Enums;
using MediatR;

namespace AqlliAgronom.Application.Features.Knowledge.Commands.CreateKnowledgeEntry;

public record CreateKnowledgeEntryCommand(
    string Title,
    string CropName,
    string ProblemName,
    ProblemCategory Category,
    string Symptoms,
    string DetailedExplanation,
    string RecommendedProducts,
    string DosagePerHectare,
    string ApplicationInstructions,
    string SafetyPrecautions,
    Language Language,
    string? GrowthStage = null,
    string? RegionConsiderations = null,
    string[]? Tags = null) : IRequest<KnowledgeEntryDto>;
