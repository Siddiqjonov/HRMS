namespace HrManager.Application.UseCases.Employees.GetEmployee;

public record GetEmployeeByIdRequest(Guid Id) : IRequest<EmployeeResponse>;
