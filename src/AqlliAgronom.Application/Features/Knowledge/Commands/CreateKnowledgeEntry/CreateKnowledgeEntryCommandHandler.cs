using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Application.Features.Knowledge.DTOs;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.Knowledge.Commands.CreateKnowledgeEntry;

public class CreateKnowledgeEntryCommandHandler(
    IUnitOfWork uow,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateKnowledgeEntryCommand, KnowledgeEntryDto>
{
    public async Task<KnowledgeEntryDto> Handle(CreateKnowledgeEntryCommand request, CancellationToken ct)
    {
        var entry = KnowledgeEntry.Create(
            title: request.Title,
            cropName: request.CropName,
            problemName: request.ProblemName,
            category: request.Category,
            symptoms: request.Symptoms,
            detailedExplanation: request.DetailedExplanation,
            recommendedProducts: request.RecommendedProducts,
            dosagePerHectare: request.DosagePerHectare,
            applicationInstructions: request.ApplicationInstructions,
            safetyPrecautions: request.SafetyPrecautions,
            language: request.Language,
            growthStage: request.GrowthStage,
            regionConsiderations: request.RegionConsiderations,
            tags: request.Tags,
            createdById: currentUser.UserId);

        await uow.KnowledgeEntries.AddAsync(entry, ct);
        await uow.SaveChangesAsync(ct);

        return MapToDto(entry);
    }

    private static KnowledgeEntryDto MapToDto(KnowledgeEntry e) => new(
        e.Id, e.Title, e.CropName, e.ProblemName, e.Category.ToString(),
        e.Symptoms, e.DetailedExplanation, e.RecommendedProducts, e.DosagePerHectare,
        e.ApplicationInstructions, e.SafetyPrecautions, e.Language.ToString(),
        e.GrowthStage, e.RegionConsiderations, e.Tags, e.Status.ToString(),
        e.Version, e.VectorId, e.CreatedAt, e.UpdatedAt);
}
