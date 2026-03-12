using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HrManager.Application.UseCases.EmployeeDocuments.DeleteEmployeeDocument;
 
public partial class DeleteEmployeeDocumentRequestHandler(
    IApplicationDbContext context,
    IStorageService storageService,
    ICurrentUserService currentUser,
    ILogger<DeleteEmployeeDocumentRequestHandler> logger)
    : IRequestHandler<DeleteEmployeeDocumentRequest>
{
    public async Task Handle(DeleteEmployeeDocumentRequest request, CancellationToken cancellationToken)
    {
        var document = await context.EmployeeDocuments
            .FirstOrDefaultAsync(d => d.Id == request.documentId, cancellationToken)
            ?? throw new NotFoundException("Document not found");

        await storageService.DeleteFileAsync(document.BlobName);
        await context.EmployeeDocuments.Where(d => d.Id == document.Id).ExecuteDeleteAsync();
        LogHardDeleted(document.Id, currentUser.Email);
        await context.SaveChangesAsync(cancellationToken);
    }
}
