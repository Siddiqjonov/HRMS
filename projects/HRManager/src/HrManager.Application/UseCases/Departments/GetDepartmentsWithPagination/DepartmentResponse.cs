using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Departments.GetDepartmentsWithPagination;

public record DepartmentResponse(Guid id, string name, string description, EmployeesBriefResponse? manager);
