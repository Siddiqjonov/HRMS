using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HrManager.Application.UseCases.EmployeeDocuments.GetEmployeeDocuments;

public class GetEmployeeDocumentsRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetEmployeeDocumentsRequest, PaginatedList<EmployeeDocumentsResponse>>
{
    public async Task<PaginatedList<EmployeeDocumentsResponse>> Handle(GetEmployeeDocumentsRequest request, CancellationToken cancellationToken)
    {
        var documentsQuery = context.EmployeeDocuments.AsNoTracking()
            .Where(d => !request.employeeId.HasValue || d.EmployeeId == request.employeeId)
            .Where(d => !request.documentType.HasValue || d.DocumentType == request.documentType);

        var responseQuery = documentsQuery.Select(d => new EmployeeDocumentsResponse(
            d.Id,
            d.FileName,
            d.DocumentType,
            Math.Round(d.FileSizeInBytes / 1024d / 1024d, 2),
            d.UploadedAt,
            d.ContentType,
            d.BlobUrl,
            (d.Employee.FirstName ?? string.Empty) + " " + (d.Employee.LastName ?? string.Empty),
            context.Employees.Where(e => e.Id == d.UpdatedBy).Select(e => e.FirstName + e.LastName).FirstOrDefault() ?? "Not defined"));

        return await PaginatedList<EmployeeDocumentsResponse>.CreateAsync(
            responseQuery, request.pageNumber, request.pageSize, cancellationToken);
    }
}