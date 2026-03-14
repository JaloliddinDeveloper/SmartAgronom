using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Events.Knowledge;

namespace AqlliAgronom.Domain.Entities;

public class KnowledgeEntry : AggregateRoot
{
    public string Title { get; private set; } = default!;
    public string CropName { get; private set; } = default!;
    public string ProblemName { get; private set; } = default!;
    public ProblemCategory Category { get; private set; }
    public string Symptoms { get; private set; } = default!;
    public string DetailedExplanation { get; private set; } = default!;
    public string RecommendedProducts { get; private set; } = default!;
    public string DosagePerHectare { get; private set; } = default!;
    public string ApplicationInstructions { get; private set; } = default!;
    public string? GrowthStage { get; private set; }
    public string? RegionConsiderations { get; private set; }
    public string SafetyPrecautions { get; private set; } = default!;
    public Language Language { get; private set; }
    public KnowledgeEntryStatus Status { get; private set; }
    public int Version { get; private set; } = 1;
    public string[] Tags { get; private set; } = Array.Empty<string>();

    // Qdrant vector ID — set after embedding is created and stored
    public Guid? VectorId { get; private set; }

    private readonly List<KnowledgeVersion> _versions = new();
    public IReadOnlyCollection<KnowledgeVersion> Versions => _versions.AsReadOnly();

    private KnowledgeEntry() { }

    public static KnowledgeEntry Create(
        string title, string cropName, string problemName,
        ProblemCategory category, string symptoms, string detailedExplanation,
        string recommendedProducts, string dosagePerHectare, string applicationInstructions,
        string safetyPrecautions, Language language,
        string? growthStage = null, string? regionConsiderations = null,
        string[]? tags = null, Guid? createdById = null)
    {
        var entry = new KnowledgeEntry
        {
            Title = title,
            CropName = cropName,
            ProblemName = problemName,
            Category = category,
            Symptoms = symptoms,
            DetailedExplanation = detailedExplanation,
            RecommendedProducts = recommendedProducts,
            DosagePerHectare = dosagePerHectare,
            ApplicationInstructions = applicationInstructions,
            SafetyPrecautions = safetyPrecautions,
            Language = language,
            GrowthStage = growthStage,
            RegionConsiderations = regionConsiderations,
            Tags = tags ?? Array.Empty<string>(),
            Status = KnowledgeEntryStatus.Draft
        };

        if (createdById.HasValue) entry.SetCreatedBy(createdById.Value);

        entry.AddDomainEvent(new KnowledgeEntryCreatedEvent(entry.Id, entry.CropName, entry.ProblemName));
        return entry;
    }

    public void Update(
        string title, string symptoms, string detailedExplanation,
        string recommendedProducts, string dosagePerHectare, string applicationInstructions,
        string safetyPrecautions, string? growthStage, string? regionConsiderations,
        string[]? tags, Guid updatedById)
    {
        var changeNote = $"Updated by user {updatedById} at {DateTime.UtcNow:O}";
        var previousContent = BuildDocument();

        _versions.Add(new KnowledgeVersion(Id, Version, previousContent, updatedById, changeNote));

        Title = title;
        Symptoms = symptoms;
        DetailedExplanation = detailedExplanation;
        RecommendedProducts = recommendedProducts;
        DosagePerHectare = dosagePerHectare;
        ApplicationInstructions = applicationInstructions;
        SafetyPrecautions = safetyPrecautions;
        GrowthStage = growthStage;
        RegionConsiderations = regionConsiderations;
        Tags = tags ?? Array.Empty<string>();
        Version++;

        // Reset VectorId so background job re-indexes
        VectorId = null;

        SetUpdatedBy(updatedById);
        AddDomainEvent(new KnowledgeEntryUpdatedEvent(Id));
    }

    public void Publish(Guid userId)
    {
        if (Status == KnowledgeEntryStatus.Archived)
            throw new DomainException("KNOWLEDGE_ARCHIVED", "Cannot publish archived knowledge entry.");
        Status = KnowledgeEntryStatus.Published;
        SetUpdatedBy(userId);
    }

    public void Archive(Guid userId)
    {
        Status = KnowledgeEntryStatus.Archived;
        VectorId = null;
        SetUpdatedBy(userId);
    }

    public void SetVectorId(Guid vectorId)
    {
        VectorId = vectorId;
        AddDomainEvent(new KnowledgeEntryIndexedEvent(Id, vectorId));
    }

    public string BuildDocument()
    {
        return $"""
            Crop: {CropName}
            Problem: {ProblemName}
            Category: {Category}
            Symptoms: {Symptoms}
            Explanation: {DetailedExplanation}
            Recommended Products: {RecommendedProducts}
            Dosage per Hectare: {DosagePerHectare}
            Application Instructions: {ApplicationInstructions}
            Growth Stage: {GrowthStage ?? "N/A"}
            Region: {RegionConsiderations ?? "General"}
            Safety: {SafetyPrecautions}
            Tags: {string.Join(", ", Tags)}
            """;
    }
}
