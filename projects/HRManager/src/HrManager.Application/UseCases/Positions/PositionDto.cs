namespace HrManager.Application.UseCases.Positions;

public record PositionDto(Guid Id, string Title, Guid DepartmentId, string DepartmentName, long SalaryMin, long SalaryMax);