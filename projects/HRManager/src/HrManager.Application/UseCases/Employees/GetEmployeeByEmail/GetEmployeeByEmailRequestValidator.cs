namespace HrManager.Application.UseCases.Employees.GetEmployeeByEmail;

public class GetEmployeeByEmailRequestValidator : AbstractValidator<GetEmployeeByEmailRequest>
{
    public GetEmployeeByEmailRequestValidator()
    {
        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .MaximumLength(50)
        .EmailAddress().WithMessage("Invalid email format");
    }
}
