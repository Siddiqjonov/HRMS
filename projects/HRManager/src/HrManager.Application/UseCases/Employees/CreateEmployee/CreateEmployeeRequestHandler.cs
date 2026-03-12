using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.CreateEmployee;

public class CreateEmployeeRequestHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateEmployeeRequest, bool>
{
    public async Task<bool> Handle(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var exists = await context.Employees
           .FirstOrDefaultAsync(e => e.PassportNumber == request.PassportNumber, cancellationToken);

        if (exists is not null)
        {
            throw new ConflictException($"Employee with passport number '{request.PassportNumber}' already exists.");
        }

        var departmentExists = await context.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (!departmentExists)
        {
            throw new NotFoundException($"Department with ID '{request.DepartmentId}' not found.");
        }

        var positionExists = await context.Positions
            .FirstOrDefaultAsync(p => p.Id == request.PositionId, cancellationToken)
            ?? throw new NotFoundException($"Position with ID '{request.PositionId}' not found.");

        if (request.Salary < positionExists.SalaryMin || request.Salary > positionExists.SalaryMax)
        {
            throw new ConflictException(
                $"Salary {request.Salary} is out of range for position '{positionExists.Title}'. " +
                $"Allowed range is between {positionExists.SalaryMin} and {positionExists.SalaryMax}.");
        }

        var scheduleExists = await context.Schedules
            .AnyAsync(s => s.Id == request.ScheduleId, cancellationToken);

        if (!scheduleExists)
        {
            throw new NotFoundException($"Schedule with ID '{request.ScheduleId}' not found.");
        }

        var employee = mapper.Map<Employee>(request);

        await context.Employees.AddAsync(employee, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
