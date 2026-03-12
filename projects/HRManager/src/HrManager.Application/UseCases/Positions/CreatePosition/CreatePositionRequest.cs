namespace HrManager.Application.UseCases.Positions.CreatePosition;

public record CreatePositionRequest(string Title, Guid DepartmentId, long SalaryMin, long SalaryMax)
    : IRequest<bool>;
