namespace HrManager.Application.UseCases.Departments.DeleteDepartment;

public class DeleteDepartmentRequestValidator : AbstractValidator<DeleteDepartmentRequest>
{
    public DeleteDepartmentRequestValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Employee ID is required.");
    }
}
