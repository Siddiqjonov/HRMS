namespace HrManager.Application.UseCases.Departments.CreateDepartment;

public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Max 100 characters");

        RuleFor(x => x.description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(250).WithMessage("Max 250 characters");
    }
}
