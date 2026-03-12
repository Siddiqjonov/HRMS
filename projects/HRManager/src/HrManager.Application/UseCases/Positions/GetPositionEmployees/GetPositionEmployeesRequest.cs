using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Positions.GetPositionEmployees;

public record GetPositionEmployeesRequest(Guid PositionId) : IRequest<IEnumerable<EmployeesBriefResponse>>;
