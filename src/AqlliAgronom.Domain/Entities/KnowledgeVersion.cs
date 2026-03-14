using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Entities;

public class KnowledgeVersion : BaseEntity
{
    public Guid KnowledgeEntryId { get; private set; }
    public int VersionNumber { get; private set; }
    public string Content { get; private set; } = default!;
    public Guid CreatedById { get; private set; }
    public string? ChangeNotes { get; private set; }

    private KnowledgeVersion() { }

    internal KnowledgeVersion(Guid entryId, int versionNumber, string content, Guid createdById, string? changeNotes)
    {
        KnowledgeEntryId = entryId;
        VersionNumber = versionNumber;
        Content = content;
        CreatedById = createdById;
        ChangeNotes = changeNotes;
    }
}
