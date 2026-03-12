using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestQueryOperations;

public record GetAbsenceRequestsWithPaginationRequest(
    Guid? EmployeeId,
    Guid? ManagerId,
    RequestStatus? Status,
    RequestType? Type,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedList<AbsenceRequestBriefInfo>>;
