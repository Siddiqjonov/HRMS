namespace HrManager.Application.UseCases.Departments.UpdateDepartment;

public record UpdateDepartmentRequest(
    Guid id,
    string name,
    string description,
    Guid? managerId
) : IRequest<bool>;
