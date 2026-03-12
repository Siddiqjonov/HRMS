namespace HrManager.Application.UseCases.Positions.UpdatePosition;

public record UpdatePositionRequest(Guid Id, string Title, Guid DepartmentId, long SalaryMin, long SalaryMax)
: IRequest<bool>;