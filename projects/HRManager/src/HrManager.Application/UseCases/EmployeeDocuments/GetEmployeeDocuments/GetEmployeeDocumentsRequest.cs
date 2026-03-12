using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.EmployeeDocuments.GetEmployeeDocuments;

public record GetEmployeeDocumentsRequest(
    Guid? employeeId,
    DocumentType? documentType = null,
    int pageNumber = 1,
    int pageSize = 10)
    : IRequest<PaginatedList<EmployeeDocumentsResponse>>;
