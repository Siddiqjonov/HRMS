using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

public class GetEmployeesWithPaginationRequestHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IRequestHandler<GetEmployeesWithPaginationRequest, PaginatedList<EmployeesBriefResponse>>
{
    public async Task<PaginatedList<EmployeesBriefResponse>> Handle(
        GetEmployeesWithPaginationRequest request, CancellationToken cancellationToken)
    {
        var query = context.Employees.AsNoTracking()
            .Select(e => new EmployeesBriefResponse
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName} {e.MiddleName}",
                DepartmentName = e.Department.Name,
                PositionName = e.Position.Title,    
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                HireDate = e.HireDate,
                IsManagerOfDepartment = context.Departments.Any(d => d.ManagerId == e.Id),
            });

        var pagedResult = await PaginatedList<EmployeesBriefResponse>.CreateAsync(
            query,
            request.pageNumber,
            request.pageSize,
            cancellationToken);

        return pagedResult;
    }
}
