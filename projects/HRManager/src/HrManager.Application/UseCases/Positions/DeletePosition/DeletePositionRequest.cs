namespace HrManager.Application.UseCases.Positions.DeletePosition;

public record DeletePositionRequest(Guid Id) : IRequest;
