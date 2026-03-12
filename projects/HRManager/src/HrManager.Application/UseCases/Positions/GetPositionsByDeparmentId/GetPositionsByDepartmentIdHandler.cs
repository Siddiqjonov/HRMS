using AutoMapper;
using AutoMapper.QueryableExtensions;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Positions.GetPositionsByDeparmentId;

public class GetPositionsByDepartmentIdHandler(
       IApplicationDbContext context,
       IMapper mapper) : IRequestHandler<GetPositionsByDepartmentIdRequest, IEnumerable<PositionDto>>
{
    public async Task<IEnumerable<PositionDto>> Handle(GetPositionsByDepartmentIdRequest request, CancellationToken cancellationToken)
    {
        var departmentExists = await context.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (!departmentExists)
        {
            throw new NotFoundException($"Department '{request.DepartmentId}' not found.");
        }

        var positions = await context.Positions
            .Where(p => p.DepartmentId == request.DepartmentId)
            .ProjectTo<PositionDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return positions;
    }
}
