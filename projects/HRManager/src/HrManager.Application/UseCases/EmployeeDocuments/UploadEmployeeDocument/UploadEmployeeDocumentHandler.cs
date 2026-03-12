using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.EmployeeDocuments.GetEmployeeDocuments;
using HrManager.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.EmployeeDocuments.UploadEmployeeDocument;

public class UploadEmployeeDocumentHandler(
    IApplicationDbContext context,
    IDateTimeService dateTime,
    IStorageService storageService,
    ICurrentUserService currentUser)
    : IRequestHandler<UploadEmployeeDocumentRequest, EmployeeDocumentsResponse>
{
    public async Task<EmployeeDocumentsResponse> Handle(UploadEmployeeDocumentRequest request, CancellationToken cancellationToken)
    {
        var employee = await context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.employeeId, cancellationToken)
            ?? throw new NotFoundException("Employee not found");

        var fileName = request.file.FileName;

        var blobName = storageService.GenerateBlobName(request.employeeId, request.documentType, fileName);

        var blobUrl = await storageService.UploadFileAsync(blobName, request.file.OpenReadStream());

        var documentDto = new EmployeeDocumentDto
        {
            EmployeeId = employee.Id,
            FileName = fileName,
            BlobName = blobName,
            FilePath = blobName,
            BlobUrl = blobUrl,
            ContainerName = "employee-documents",
            FileSizeInBytes = request.file.Length,
            ContentType = request.file.ContentType,
            DocumentType = request.documentType,
            UploadedAt = dateTime.UtcNow,
            UploadedByUserId = currentUser.UserId,
        };

        var document = new EmployeeDocument(documentDto);

        await context.EmployeeDocuments.AddAsync(document, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new EmployeeDocumentsResponse(
            document.Id,
            document.FileName,
            document.DocumentType,
            document.FileSizeInBytes,
            document.UploadedAt,
            document.ContentType!,
            document.BlobUrl!,
            (document.Employee.FirstName ?? string.Empty) + " " + (document.Employee.LastName ?? string.Empty),
            await context.Employees.Where(e => e.Id == document.UpdatedBy).Select(e => e.FirstName + e.LastName).FirstOrDefaultAsync(cancellationToken) ?? "Not defined");
    }
}
