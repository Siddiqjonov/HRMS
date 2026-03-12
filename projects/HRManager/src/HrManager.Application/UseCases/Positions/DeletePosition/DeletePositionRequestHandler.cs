using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Positions.DeletePosition;

public class DeletePositionRequestHandler(
    IApplicationDbContext context) : IRequestHandler<DeletePositionRequest>
{
    public async Task Handle(DeletePositionRequest request, CancellationToken cancellationToken)
    {
        var position = await context.Positions.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
           ?? throw new NotFoundException($"Position '{request.Id}' not found.");

        var hasEmployees = await context.Employees.AnyAsync(e => e.PositionId == request.Id, cancellationToken);
        if (hasEmployees)
        {
            throw new ConflictException("Cannot delete position because employees are assigned to it.");
        }

        context.Positions.Remove(position);

        await context.SaveChangesAsync(cancellationToken);
    }
}
