namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendanceRecordsWithFilter;

public class GetAttendanceRecordsRequestValidator : AbstractValidator<GetAttendanceRecordsRequest>
{
    public GetAttendanceRecordsRequestValidator()
    {
        RuleFor(x => x.pageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.pageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be less than or equal to 100.");
    }
}
