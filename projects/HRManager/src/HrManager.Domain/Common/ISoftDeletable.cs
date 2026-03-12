namespace HrManager.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }

    DateTime? DeletedOnUtc { get; set; }

    Guid? DeletedBy { get; set; }
}