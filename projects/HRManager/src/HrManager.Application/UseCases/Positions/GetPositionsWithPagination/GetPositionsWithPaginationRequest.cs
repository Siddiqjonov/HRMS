using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Positions.GetPositionsWithPagination;

public record GetPositionsWithPaginationRequest(
 int pageNumber = 1,
 int pageSize = 10)
 : IRequest<PaginatedList<PositionDto>>;