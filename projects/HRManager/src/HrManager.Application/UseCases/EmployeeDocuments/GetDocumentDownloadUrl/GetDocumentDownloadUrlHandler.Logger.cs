using Microsoft.Extensions.Logging;

namespace HrManager.Application.UseCases.EmployeeDocuments.GetDocumentDownloadUrl;

public partial class GetDocumentDownloadUrlHandler
{
    [LoggerMessage(
        0,
        LogLevel.Information,
        "Started generating SAS URL for EmployeeId {EmployeeId}, DocumentId {DocumentId}"
    )]
    partial void LogAccessStarted(Guid EmployeeId, Guid DocumentId);

    [LoggerMessage(
        1,
        LogLevel.Information,
        "Successfully generated SAS URL for EmployeeId {EmployeeId}, DocumentId {DocumentId}"
    )]
    partial void LogAccessCompleted(Guid EmployeeId, Guid DocumentId);
}
