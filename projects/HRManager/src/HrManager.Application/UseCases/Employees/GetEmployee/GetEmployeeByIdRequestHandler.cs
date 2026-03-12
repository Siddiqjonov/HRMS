using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.GetEmployee;

public class GetEmployeeByIdRequestHandler(
IApplicationDbContext context,
IMapper mapper) : IRequestHandler<GetEmployeeByIdRequest, EmployeeResponse>
{
    public async Task<EmployeeResponse> Handle(GetEmployeeByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Employees
                .AsNoTracking()
                .Where(e => e.Id == request.Id)
                .ProjectTo<EmployeeResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException($"Employee with ID {request.Id} not found.");
        }

        return result;
    }
}
