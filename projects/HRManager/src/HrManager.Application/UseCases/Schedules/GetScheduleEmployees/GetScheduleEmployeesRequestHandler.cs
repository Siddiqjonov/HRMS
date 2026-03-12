using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Schedules.GetScheduleEmployees;

public class GetScheduleEmployeesRequestHandler(
 IApplicationDbContext context,
 IMapper mapper) : IRequestHandler<GetScheduleEmployeesRequest, IEnumerable<EmployeesBriefResponse>>
{
    public async Task<IEnumerable<EmployeesBriefResponse>> Handle(GetScheduleEmployeesRequest request, CancellationToken cancellationToken)
    {
        _ = await context.Schedules
            .FirstOrDefaultAsync(s => s.Id == request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException($"Schedule '{request.ScheduleId}' not found.");

        var employees = await context.Employees
            .Where(e => e.ScheduleId == request.ScheduleId)
            .Select(e => new EmployeesBriefResponse
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName} {e.MiddleName}",
                DepartmentName = e.Department.Name,
                PositionName = e.Position.Title,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                HireDate = e.HireDate,
            })
            .ToListAsync(cancellationToken);

        return employees;
    }
}
