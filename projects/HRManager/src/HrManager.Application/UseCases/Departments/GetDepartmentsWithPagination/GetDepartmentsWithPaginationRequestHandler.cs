using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Departments.GetDepartmentsWithPagination;

public class GetDepartmentsWithPaginationRequestHandler(
    IApplicationDbContext context) : IRequestHandler<GetDepartmentsWithPaginationRequest, PaginatedList<DepartmentResponse>>
{
    public async Task<PaginatedList<DepartmentResponse>> Handle(GetDepartmentsWithPaginationRequest request, CancellationToken cancellationToken)
    {
        var responseDepartments = context.Departments
            .Select(d => new DepartmentResponse(
                d.Id, d.Name, d.Description, d.Manager == null ? null :
                new EmployeesBriefResponse
                {
                    Id = d.Manager.Id,
                    FullName = $"{d.Manager.FirstName} {d.Manager.LastName} {d.Manager.MiddleName}",
                    DepartmentName = d.Name,
                    PositionName = d.Manager.Position.Title,
                    Email = d.Manager.Email,
                    PhoneNumber = d.Manager.PhoneNumber,
                    HireDate = d.Manager.HireDate,
                }));

        var pagedResult = await PaginatedList<DepartmentResponse>.CreateAsync(
            responseDepartments,
            request.pageNumber,
            request.pageSize,
            cancellationToken);

        return pagedResult;
    }
}
