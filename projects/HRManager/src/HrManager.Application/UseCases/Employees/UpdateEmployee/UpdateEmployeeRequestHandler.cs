using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.UpdateEmployee;

public class UpdateEmployeeRequestHandler(
IApplicationDbContext context,
IMapper mapper
) : IRequestHandler<UpdateEmployeeRequest, bool>
{
    public async Task<bool> Handle(UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException($"Employee with ID {request.Id} not found.");
        }

        mapper.Map(request, employee);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
