namespace HrManager.Application.UseCases.Departments.GetDepartmentOverview;

public record GetDepartmentOverviewRequest(Guid departmentId)
    : IRequest<DepartmentOverviewResponse>;
