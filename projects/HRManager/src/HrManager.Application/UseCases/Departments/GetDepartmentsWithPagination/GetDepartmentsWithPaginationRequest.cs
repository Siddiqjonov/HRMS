using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Departments.GetDepartmentsWithPagination;

public record GetDepartmentsWithPaginationRequest(
    int pageNumber = 1,
    int pageSize = 10) : IRequest<PaginatedList<DepartmentResponse>>;
