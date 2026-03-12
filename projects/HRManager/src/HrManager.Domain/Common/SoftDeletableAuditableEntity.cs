namespace HrManager.Domain.Common;

public abstract class SoftDeletableAuditableEntity : AuditableEntity<Guid>, ISoftDeletable
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedOnUtc { get; set; }

    public Guid? DeletedBy { get; set; }
}