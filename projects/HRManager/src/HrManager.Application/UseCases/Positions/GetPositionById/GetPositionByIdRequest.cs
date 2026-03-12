namespace HrManager.Application.UseCases.Positions.GetPositionById;

public record GetPositionById(Guid Id) : IRequest<PositionDto>;
