using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.DeleteDepartment;

public class DeleteDepartmentRequestHandler(
    IApplicationDbContext context) : IRequestHandler<DeleteDepartmentRequest>
{
    public async Task Handle(DeleteDepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.id, cancellationToken)
            ?? throw new NotFoundException($"Department with given id {request.id} not found");

        var hasEmployees = await context.Employees
            .AnyAsync(e => e.DepartmentId == request.id, cancellationToken);

        if (hasEmployees)
        {
            throw new ConflictException("Cannot delete department with active employees");
        }   

        context.Departments.Remove(department);
        await context.SaveChangesAsync(cancellationToken);
    }
}
