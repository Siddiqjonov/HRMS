using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Positions.GetPositionEmployees;

public class GetPositionEmployeesRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetPositionEmployeesRequest, IEnumerable<EmployeesBriefResponse>>
{
    public async Task<IEnumerable<EmployeesBriefResponse>> Handle(GetPositionEmployeesRequest request, CancellationToken cancellationToken)
    {
        var position = await context.Positions.FirstOrDefaultAsync(p => p.Id == request.PositionId, cancellationToken)
                          ?? throw new NotFoundException($"Position '{request.PositionId}' not found.");

        List<EmployeesBriefResponse> employees = await context.Employees
            .Where(e => e.PositionId == request.PositionId)
            .Select(e => new EmployeesBriefResponse
            {
                Id = e.Id,
                FullName = e.FirstName + " " + e.LastName,
                DepartmentName = e.Department.Name,
                PositionName = e.Position.Title,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                HireDate = e.HireDate,
            }).ToListAsync(cancellationToken);

        return employees;
    }
}
