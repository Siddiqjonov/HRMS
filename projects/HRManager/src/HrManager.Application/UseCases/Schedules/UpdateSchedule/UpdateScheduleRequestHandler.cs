using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Schedules.UpdateSchedule;

public class UpdateScheduleRequestHandler(
IApplicationDbContext context,
IMapper mapper) : IRequestHandler<UpdateScheduleRequest, bool>
{
    public async Task<bool> Handle(UpdateScheduleRequest request, CancellationToken cancellationToken)
    {
        var schedule = await context.Schedules
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Schedule '{request.Id}' not found.");

        var nameExists = await context.Schedules
            .AnyAsync(s => s.Name == request.Name && s.Id != request.Id, cancellationToken);

        if (nameExists)
        {
            throw new ConflictException($"Schedule with name '{request.Name}' already exists.");
        }

        mapper.Map(request, schedule);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
