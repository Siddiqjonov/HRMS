namespace HrManager.Application.UseCases.Departments.RemoveDepartmentManager;

public record RemoveDepartmentManagerRequest(Guid departmentId) : IRequest;
