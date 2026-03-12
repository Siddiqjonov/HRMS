namespace HrManager.Application.UseCases.Departments.AssignDepartmentManager;

public record AssignDepartmentManagerRequest(Guid departmentId, Guid employeeId) : IRequest;
