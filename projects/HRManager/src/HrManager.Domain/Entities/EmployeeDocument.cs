using HrManager.Domain.Dtos;

namespace HrManager.Domain.Entities;

public class EmployeeDocument : SoftDeletableAuditableEntity
{
    public EmployeeDocument() { }

    public EmployeeDocument(EmployeeDocumentDto dto)
    {
        EmployeeId = dto.EmployeeId;
        FileName = dto.FileName;
        FilePath = dto.FilePath;
        FileSizeInBytes = dto.FileSizeInBytes;
        ContentType = dto.ContentType;
        BlobName = dto.BlobName;
        BlobUrl = dto.BlobUrl;
        ContainerName = dto.ContainerName;
        DocumentType = dto.DocumentType;
        UploadedAt = dto.UploadedAt;
        UploadedByUserId = dto.UploadedByUserId;
    }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; private set; }

    public string FileName { get; private set; }

    public string FilePath { get; private set; }

    public string BlobName { get; private set; }

    public string? BlobUrl { get; private set; }

    public string ContainerName { get; private set; }

    public long FileSizeInBytes { get; private set; }

    public string? ContentType { get; private set; }

    public DocumentType DocumentType { get; private set; }

    public DateTime UploadedAt { get; private set; }

    public Guid UploadedByUserId { get; private set; }
}
