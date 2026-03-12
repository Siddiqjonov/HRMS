namespace HrManager.Application.UseCases.EmployeeDocuments.GetDocumentDownloadUrl;

public record GetDocumentDownloadUrlRequest(Guid documentId)
    : IRequest<GetDocumentDownloadUrlResponse>;
