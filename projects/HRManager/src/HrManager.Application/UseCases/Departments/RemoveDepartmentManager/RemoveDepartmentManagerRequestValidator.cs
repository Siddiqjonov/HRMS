namespace HrManager.Application.UseCases.Departments.RemoveDepartmentManager;

public class RemoveDepartmentManagerRequestValidator : AbstractValidator<RemoveDepartmentManagerRequest>
{
    public RemoveDepartmentManagerRequestValidator()
    {
        RuleFor(x => x.departmentId)
            .NotEmpty().WithMessage("Department Id is required");
    }
}
