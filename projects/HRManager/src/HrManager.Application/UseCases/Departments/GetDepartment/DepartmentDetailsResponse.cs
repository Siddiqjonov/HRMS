using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Departments.GetDepartment;

public record DepartmentDetailsResponse(Guid id, string name, string description, int employeeCount, EmployeesBriefResponse? manager);
