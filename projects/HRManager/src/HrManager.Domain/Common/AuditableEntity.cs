namespace HrManager.Domain.Common
{
    public class AuditableEntity<T> : BaseEntity<T>, IAuditable
    {
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }
    }
}