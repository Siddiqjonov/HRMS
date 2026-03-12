using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.GetEmployeeByEmail;

public class GetEmployeeByEmailRequestHandler(
IApplicationDbContext context,
IMapper mapper) : IRequestHandler<GetEmployeeByEmailRequest, EmployeeResponse>
{
    public async Task<EmployeeResponse> Handle(GetEmployeeByEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Employees
            .AsNoTracking()
            .Where(e => e.Email == request.Email)
            .ProjectTo<EmployeeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"Employee with email '{request.Email}' not found.");
        }

        result.IsManagerOfDepartment = await context.Departments
            .AsNoTracking()
            .AnyAsync(d => d.ManagerId == result.Id, cancellationToken);

        return result;
    }
}
