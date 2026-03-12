using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.EmployeeDocuments.GetEmployeeDocuments;

public record EmployeeDocumentsResponse(
    Guid id,
    string fileName,
    DocumentType documentType,
    double fileSizeInMb,
    DateTime uploadedAt,
    string? contentType,
    string? blobUrl,
    string employeeName,
    string uploadedBy);
