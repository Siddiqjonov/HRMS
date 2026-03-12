using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.UpdateDepartment;

public class UpdateDepartmentRequestHandler(
    IApplicationDbContext context,
    IMapper mapper) : IRequestHandler<UpdateDepartmentRequest, bool>
{
    public async Task<bool> Handle(UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var departmentFromDb = await context.Departments
            .FirstOrDefaultAsync(d => d.Name == request.name && d.Id != request.id, cancellationToken: cancellationToken);

        if (departmentFromDb is not null)
        {
            throw new ConflictException($"Department '{request.name}' already exists.");
        }

        if (request.managerId is not null)
        {
            var hasManager = await context.Departments
                .AnyAsync(d => d.ManagerId == request.managerId && d.Id != request.id, cancellationToken);

            if (hasManager)
            {
                throw new ConflictException($"Manager '{request.managerId}' is already assigned to another department.");
            }
        }

        var department = await context.Departments
        .FirstOrDefaultAsync(d => d.Id == request.id, cancellationToken)
            ?? throw new NotFoundException($"Department '{request.id}' not found.");

        mapper.Map(request, department);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
