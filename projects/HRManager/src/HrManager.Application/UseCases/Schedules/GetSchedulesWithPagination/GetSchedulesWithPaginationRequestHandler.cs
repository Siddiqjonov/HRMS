using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Schedules.GetSchedulesWithPagination
{
    public class GetSchedulesWithPaginationRequestHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IRequestHandler<GetSchedulesWithPaginationRequest, PaginatedList<ScheduleDto>>
    {
        public async Task<PaginatedList<ScheduleDto>> Handle(
            GetSchedulesWithPaginationRequest request,
            CancellationToken cancellationToken)
        {
            var query = context.Schedules.AsNoTracking();

            var pagedResult = await PaginatedList<ScheduleDto>.CreateAsync(
                query.ProjectTo<ScheduleDto>(mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return pagedResult;
        }
    }
}
