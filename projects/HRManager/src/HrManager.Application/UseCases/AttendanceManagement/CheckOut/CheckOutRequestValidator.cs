namespace HrManager.Application.UseCases.AttendanceManagement.CheckOut;

public class CheckOutRequestValidator : AbstractValidator<CheckOutRequest>
{
    public CheckOutRequestValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee Id is required");
    }
}
