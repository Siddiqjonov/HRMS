namespace HrManager.Application.UseCases.Departments.AssignDepartmentManager;

public class AssignDepartmentManagerRequestValidator : AbstractValidator<AssignDepartmentManagerRequest>
{
    public AssignDepartmentManagerRequestValidator()
    {
        RuleFor(x => x.employeeId)
            .NotEmpty().WithMessage("Employee ID is required");
        RuleFor(x => x.departmentId)
            .NotEmpty().WithMessage("Department ID is required");
    }
}
