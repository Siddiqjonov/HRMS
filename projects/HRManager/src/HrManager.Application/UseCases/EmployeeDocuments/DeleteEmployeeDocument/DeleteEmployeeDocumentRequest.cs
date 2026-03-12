namespace HrManager.Application.UseCases.EmployeeDocuments.DeleteEmployeeDocument;

public record DeleteEmployeeDocumentRequest(
    Guid documentId)
    : IRequest;
