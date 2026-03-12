using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.AssignDepartmentManager;

public class AssignDepartmentManagerRequestHandler(
    IApplicationDbContext context) : IRequestHandler<AssignDepartmentManagerRequest>
{
    public async Task Handle(AssignDepartmentManagerRequest request, CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.departmentId, cancellationToken)
            ?? throw new NotFoundException($"Department with ID '{request.departmentId}' not found.");

        var employee = await context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.employeeId, cancellationToken)
            ?? throw new NotFoundException($"Employee with ID '{request.employeeId}' not found.");

        var alreadyManager = await context.Departments
            .AnyAsync(d => d.ManagerId == request.employeeId && d.Id != request.departmentId, cancellationToken);

        if (alreadyManager)
        {
            throw new ConflictException($"Employee '{request.employeeId}' is already assigned as a manager of another department.");
        }

        department.ManagerId = employee.Id;

        await context.SaveChangesAsync(cancellationToken);
    }
}
