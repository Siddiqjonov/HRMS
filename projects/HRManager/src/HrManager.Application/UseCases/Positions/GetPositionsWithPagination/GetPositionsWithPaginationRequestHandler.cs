using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Positions.GetPositionsWithPagination;

public class GetPositionsWithPaginationRequestHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IRequestHandler<GetPositionsWithPaginationRequest, PaginatedList<PositionDto>>
{
    public async Task<PaginatedList<PositionDto>> Handle(
        GetPositionsWithPaginationRequest request,
        CancellationToken cancellationToken)
    {
        var query = context.Positions
            .Include(p => p.Department)
            .AsNoTracking();

        var pagedResult = await PaginatedList<PositionDto>.CreateAsync(
            query.ProjectTo<PositionDto>(mapper.ConfigurationProvider),
            request.pageNumber,
            request.pageSize,
            cancellationToken);

        return pagedResult;
    }
}
