namespace HrManager.Application.UseCases.Employees.DeleteEmployee;

public record DeleteEmployeeRequest(Guid Id) : IRequest;
