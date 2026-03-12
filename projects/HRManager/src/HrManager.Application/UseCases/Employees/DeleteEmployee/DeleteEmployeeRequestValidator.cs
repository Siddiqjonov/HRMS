namespace HrManager.Application.UseCases.Employees.DeleteEmployee;

public class DeleteEmployeeRequestValidator : AbstractValidator<DeleteEmployeeRequest>
{
    public DeleteEmployeeRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
    }
}
