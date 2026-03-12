namespace HrManager.Application.UseCases.AttendanceManagement.CheckIn;

public class CheckInRequestValidator : AbstractValidator<CheckInRequest>
{
    public CheckInRequestValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee Id is required.");
    }
}
