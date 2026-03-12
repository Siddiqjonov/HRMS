namespace HrManager.Application.UseCases.Departments.GetDepartmentEmployees;

public class GetDepartmentEmployeesRequestValidator : AbstractValidator<GetDepartmentEmployeesRequest>
{
    public GetDepartmentEmployeesRequestValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("Department ID is required.");
    }
}
