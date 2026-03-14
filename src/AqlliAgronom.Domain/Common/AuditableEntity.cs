namespace AqlliAgronom.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public Guid? CreatedById { get; protected set; }
    public Guid? UpdatedById { get; protected set; }

    protected void SetCreatedBy(Guid userId) => CreatedById = userId;
    protected void SetUpdatedBy(Guid userId)
    {
        UpdatedById = userId;
        SetUpdatedAt();
    }
}
