using AutoMapper;
using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Positions.GetPositionById
{
    public class GetPositionByIdRequestHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<GetPositionById, PositionDto>
    {
        public async Task<PositionDto> Handle(GetPositionById request, CancellationToken cancellationToken)
        {
            var position = await context.Positions
                .Include(p => p.Department).FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException($"Position '{request.Id}' not found.");

            return mapper.Map<PositionDto>(position);
        }
    }
}
