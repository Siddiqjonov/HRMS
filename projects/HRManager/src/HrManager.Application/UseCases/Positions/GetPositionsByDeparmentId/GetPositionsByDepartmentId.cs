namespace HrManager.Application.UseCases.Positions.GetPositionsByDeparmentId;

public record GetPositionsByDepartmentIdRequest(Guid DepartmentId) : IRequest<IEnumerable<PositionDto>>;
