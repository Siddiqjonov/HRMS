namespace HrManager.Application.UseCases.Departments.GetDepartment;

public class GetDepartmentByIdRequestValidator : AbstractValidator<GetDepartmentByIdRequest>
{
    public GetDepartmentByIdRequestValidator()
    {
        RuleFor(x => x.id)
           .NotEmpty().WithMessage("Employee ID is required.");
    }
}
