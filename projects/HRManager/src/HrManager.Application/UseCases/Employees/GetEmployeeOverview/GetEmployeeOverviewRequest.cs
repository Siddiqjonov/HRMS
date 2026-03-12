namespace HrManager.Application.UseCases.Employees.GetEmployeeOverview;

public record GetEmployeeOverviewRequest()
    : IRequest<EmployeeOverviewResponse>;
