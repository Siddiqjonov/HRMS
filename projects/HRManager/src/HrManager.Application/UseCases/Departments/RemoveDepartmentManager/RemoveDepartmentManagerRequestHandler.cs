using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.RemoveDepartmentManager;

public class RemoveDepartmentManagerRequestHandler(
    IApplicationDbContext context) : IRequestHandler<RemoveDepartmentManagerRequest>
{
    public async Task Handle(RemoveDepartmentManagerRequest request, CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.departmentId, cancellationToken)
            ?? throw new NotFoundException($"Department with ID {request.departmentId} not found");

        if (department.ManagerId is not null)
        {
            department.ManagerId = null;
        }

        context.Departments.Update(department);
        await context.SaveChangesAsync(cancellationToken);
    }
}
