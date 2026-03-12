using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Schedules.CreateSchedule;

public class CreateScheduleRequestHandler(
IApplicationDbContext context,
IMapper mapper) : IRequestHandler<CreateScheduleRequest, ScheduleDto>
{
    public async Task<ScheduleDto> Handle(CreateScheduleRequest request, CancellationToken cancellationToken)
    {
        var exists = await context.Schedules
           .AnyAsync(s => s.Name == request.Name, cancellationToken);

        if (exists)
        {
            throw new ConflictException($"Schedule with name '{request.Name}' already exists.");
        }

        var schedule = mapper.Map<Schedule>(request);

        await context.Schedules.AddAsync(schedule, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = mapper.Map<ScheduleDto>(schedule);

        return result;
    }
}