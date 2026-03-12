using Microsoft.Extensions.Logging;

namespace HrManager.Application.UseCases.EmployeeDocuments.DeleteEmployeeDocument;

public partial class DeleteEmployeeDocumentRequestHandler
{
    [LoggerMessage(
        0,
        LogLevel.Information,
        "[EmployeeDocuments] Soft deleted DocumentId {DocumentId} by User {userEmail}"
    )]
    partial void LogSoftDeleted(Guid DocumentId, string userEmail);

    [LoggerMessage(
        1,
        LogLevel.Information,
        "[EmployeeDocuments] Hard deleted DocumentId {DocumentId} by Admin {userEmail}"
    )]
    partial void LogHardDeleted(Guid DocumentId, string userEmail);
}
