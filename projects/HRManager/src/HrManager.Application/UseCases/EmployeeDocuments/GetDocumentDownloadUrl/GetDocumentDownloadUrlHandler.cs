using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HrManager.Application.UseCases.EmployeeDocuments.GetDocumentDownloadUrl;

public partial class GetDocumentDownloadUrlHandler(
    IApplicationDbContext _context,
    IStorageService _storageService,
    ILogger<GetDocumentDownloadUrlHandler> _logger) : IRequestHandler<GetDocumentDownloadUrlRequest, GetDocumentDownloadUrlResponse>

{
    public async Task<GetDocumentDownloadUrlResponse> Handle(GetDocumentDownloadUrlRequest request, CancellationToken cancellationToken)
    {
        var document = await _context.EmployeeDocuments
            .FirstOrDefaultAsync(d => d.Id == request.documentId, cancellationToken)
            ?? throw new NotFoundException("Document not found");

        LogAccessStarted(document.EmployeeId, document.Id);

        var expiry = TimeSpan.FromHours(1);
        var sasUri = await _storageService.GetSasUriAsync(document.BlobName, expiry);

        LogAccessCompleted(document.EmployeeId, document.Id);

        return new GetDocumentDownloadUrlResponse(sasUri);
    }
}
