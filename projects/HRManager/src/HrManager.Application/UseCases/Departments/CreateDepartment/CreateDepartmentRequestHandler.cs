using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.CreateDepartment;

public class CreateDepartmentRequestHandler(
    IApplicationDbContext context,
    IMapper mapper) : IRequestHandler<CreateDepartmentRequest, bool>
{
    public async Task<bool> Handle(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var managerExists = await context.Employees
        .AnyAsync(e => e.Id == request.managerId, cancellationToken);

        if (!managerExists && request.managerId is not null)
        {
            throw new NotFoundException($"Manager with ID '{request.managerId}' does not exist.");
        }

        var department = await context.Departments
            .FirstOrDefaultAsync(d => d.Name == request.name, cancellationToken: cancellationToken);

        if (department is not null)
        {
            throw new ConflictException($"Department '{request.name}' already exists.");
        }

        if (request.managerId is not null)
        {
            var hasManager = await context.Departments
                .AnyAsync(d => d.ManagerId == request.managerId, cancellationToken);

            if (hasManager)
            {
                throw new ConflictException($"Manager '{request.managerId}' is already assigned to another department.");
            }
        }

        department = mapper.Map<Department>(request);

        await context.Departments.AddAsync(department, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
