using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Schedules.GetScheduleEmployees;

public record GetScheduleEmployeesRequest(Guid ScheduleId) : IRequest<IEnumerable<EmployeesBriefResponse>>;
