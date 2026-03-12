using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Schedules.GetSchedulesWithPagination;

public record GetSchedulesWithPaginationRequest(
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PaginatedList<ScheduleDto>>;