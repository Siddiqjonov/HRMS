namespace HrManager.Application.UseCases.Departments.GetDepartment;

public record GetDepartmentByIdRequest(Guid id) : IRequest<DepartmentDetailsResponse>;
