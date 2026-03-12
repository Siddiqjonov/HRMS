using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Positions.CreatePosition;

public class CreatePositionRequestHandler(
    IApplicationDbContext context,
    IMapper mapper) : IRequestHandler<CreatePositionRequest, bool>
{
    public async Task<bool> Handle(CreatePositionRequest request, CancellationToken cancellationToken)
    {
        _ = await context.Departments.FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken)
             ?? throw new NotFoundException($"Department '{request.DepartmentId}' not found.");

        var position = mapper.Map<Position>(request);

        await context.Positions.AddAsync(position, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
