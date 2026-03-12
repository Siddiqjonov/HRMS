using FluentValidation;

namespace HrManager.Application.UseCases.Departments.GetDepartmentOverview;

public class GetDepartmentOverviewRequestValidator : AbstractValidator<GetDepartmentOverviewRequest>
{
    public GetDepartmentOverviewRequestValidator()
    {
        RuleFor(x => x.departmentId)
            .NotEmpty()
            .WithMessage("Department ID is required.");
    }
}
