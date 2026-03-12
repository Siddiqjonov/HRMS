using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.GetDepartmentEmployees;

public class GetDepartmentEmployeesRequestHandler(
    IApplicationDbContext context) : IRequestHandler<GetDepartmentEmployeesRequest, List<EmployeesBriefResponse>>
{
    public async Task<List<EmployeesBriefResponse>> Handle(GetDepartmentEmployeesRequest request, CancellationToken cancellationToken)
    {
        var employees = await context.Employees
            .AsNoTracking()
            .Where(e => e.DepartmentId == request.DepartmentId)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Select(e => new EmployeesBriefResponse
            {
                Id = e.Id,
                FullName = e.FirstName + " " + e.LastName,
                DepartmentName = e.Department != null ? e.Department.Name : string.Empty,
                PositionName = e.Position != null ? e.Position.Title : string.Empty,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                HireDate = e.HireDate,
                IsManagerOfDepartment = context.Departments.Any(d => d.ManagerId == e.Id),
            })
            .ToListAsync(cancellationToken);

        return employees;
    }
}
