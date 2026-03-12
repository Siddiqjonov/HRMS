namespace HrManager.Application.UseCases.Departments.UpdateDepartment;

public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Employee ID is required.");

        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Max 100 characters");

        RuleFor(x => x.description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(250).WithMessage("Max 250 characters");
    }
}
