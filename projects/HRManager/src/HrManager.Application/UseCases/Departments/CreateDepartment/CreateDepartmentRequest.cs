namespace HrManager.Application.UseCases.Departments.CreateDepartment;

public record CreateDepartmentRequest(
    string name,
    string description,
    Guid? managerId
) : IRequest<bool>;
