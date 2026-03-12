using HrManager.Application.UseCases.Employees.GetEmployee;

public class GetEmployeeByIdRequestValidator : AbstractValidator<GetEmployeeByIdRequest>
{
    public GetEmployeeByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Employee ID is required.");
    }
}