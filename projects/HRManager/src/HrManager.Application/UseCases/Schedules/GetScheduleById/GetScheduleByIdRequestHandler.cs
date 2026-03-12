using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Schedules.GetScheduleById;

public class GetScheduleByIdRequestHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<GetScheduleByIdRequest, ScheduleDto>
{
    public async Task<ScheduleDto> Handle(GetScheduleByIdRequest request, CancellationToken cancellationToken)
    {
        var schedule = await context.Schedules.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Schedule '{request.Id}' not found.");

        return mapper.Map<ScheduleDto>(schedule);
    }
}
