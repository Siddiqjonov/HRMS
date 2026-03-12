namespace HrManager.Domain.Common;

public interface IAuditable
{
    Guid CreatedBy { get; set; }

    DateTime CreatedOnUtc { get; set; }

    Guid? UpdatedBy { get; set; }

    DateTime? UpdatedOnUtc { get; set; }
}