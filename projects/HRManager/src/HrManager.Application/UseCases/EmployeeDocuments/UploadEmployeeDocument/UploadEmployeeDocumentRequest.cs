using HrManager.Application.UseCases.EmployeeDocuments.GetEmployeeDocuments;
using HrManager.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrManager.Application.UseCases.EmployeeDocuments.UploadEmployeeDocument;

public record UploadEmployeeDocumentRequest(
    Guid employeeId,
    IFormFile file,
    DocumentType documentType)
    : IRequest<EmployeeDocumentsResponse>;
