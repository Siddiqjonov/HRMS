namespace HrManager.Domain.Dtos;

public class EmployeeDocumentDto
{
    public Guid EmployeeId { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }

    public long FileSizeInBytes { get; set; }

    public DateTime UploadedAt { get; set; }

    public string BlobName { get; set; }

    public string? BlobUrl { get; set; }

    public string ContainerName { get; set; }

    public string? ContentType { get; set; }

    public DocumentType DocumentType { get; set; }

    public Guid UploadedByUserId { get; set; }
}
