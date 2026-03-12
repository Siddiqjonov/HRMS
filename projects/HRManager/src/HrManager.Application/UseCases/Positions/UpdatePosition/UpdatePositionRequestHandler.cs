using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Positions.UpdatePosition;

internal class UpdatePositionRequestHandler(
    IApplicationDbContext context,
    IMapper mapper) : IRequestHandler<UpdatePositionRequest, bool>
{
    public async Task<bool> Handle(UpdatePositionRequest request, CancellationToken cancellationToken)
    {
        var position = await context.Positions.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Position '{request.Id}' not found.");

        _ = await context.Departments.FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken)
            ?? throw new NotFoundException($"Department '{request.DepartmentId}' not found.");

        mapper.Map(request, position);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
