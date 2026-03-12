using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Schedules.DeleteSchedule;

public class DeleteScheduleRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<DeleteScheduleRequest>
{
    public async Task Handle(DeleteScheduleRequest request, CancellationToken cancellationToken)
    {
        var schedule = await context.Schedules
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Schedule '{request.Id}' not found.");

        var hasEmployees = await context.Employees
            .AnyAsync(e => e.ScheduleId == request.Id, cancellationToken);

        if (hasEmployees)
        {
            throw new ConflictException("Cannot delete schedule because employees are assigned to it.");
        }

        context.Schedules.Remove(schedule);
        await context.SaveChangesAsync(cancellationToken);
    }
}
