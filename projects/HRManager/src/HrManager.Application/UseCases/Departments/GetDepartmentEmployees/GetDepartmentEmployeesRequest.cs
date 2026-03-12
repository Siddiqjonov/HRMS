using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Departments.GetDepartmentEmployees;

public record GetDepartmentEmployeesRequest(Guid DepartmentId) : IRequest<List<EmployeesBriefResponse>>;
