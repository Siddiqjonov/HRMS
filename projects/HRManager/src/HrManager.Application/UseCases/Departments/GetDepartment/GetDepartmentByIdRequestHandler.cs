using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.GetDepartment;

public class GetDepartmentByIdRequestHandler(
    IApplicationDbContext context) : IRequestHandler<GetDepartmentByIdRequest, DepartmentDetailsResponse>
{
    public async Task<DepartmentDetailsResponse> Handle(GetDepartmentByIdRequest request, CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .Where(d => d.Id == request.id)
            .Select(d => new DepartmentDetailsResponse(
                d.Id, d.Name, d.Description, d.Employees.Count(), d.Manager == null ? null :
                new EmployeesBriefResponse
                {
                    Id = d.Manager.Id,
                    FullName = d.Manager.FirstName + " " + d.Manager.LastName,
                    DepartmentName = d.Manager.Department.Name,
                    PositionName = d.Manager.Position.Title,
                    Email = d.Manager.Email,
                    PhoneNumber = d.Manager.PhoneNumber,
                    HireDate = d.Manager.HireDate
                }
            ))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Department with id {request.id} not found");

        return department;
    }
}
