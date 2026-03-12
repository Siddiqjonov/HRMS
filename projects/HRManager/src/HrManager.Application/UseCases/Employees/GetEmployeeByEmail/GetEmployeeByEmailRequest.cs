namespace HrManager.Application.UseCases.Employees.GetEmployeeByEmail;

public record GetEmployeeByEmailRequest(string Email) : IRequest<EmployeeResponse>;
