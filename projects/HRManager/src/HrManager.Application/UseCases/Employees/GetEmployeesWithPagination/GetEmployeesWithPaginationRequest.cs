namespace HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

public record GetEmployeesWithPaginationRequest(
    int pageNumber = 1, 
    int pageSize = 10)
    : IRequest<PaginatedList<EmployeesBriefResponse>>;