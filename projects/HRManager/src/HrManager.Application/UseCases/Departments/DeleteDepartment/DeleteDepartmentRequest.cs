namespace HrManager.Application.UseCases.Departments.DeleteDepartment;

public record DeleteDepartmentRequest(Guid id) : IRequest;