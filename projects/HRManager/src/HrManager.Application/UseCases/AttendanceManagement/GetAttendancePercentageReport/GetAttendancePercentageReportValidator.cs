namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendancePercentageReport;

public class GetAttendancePercentageReportValidator : AbstractValidator<GetAttendancePercentageReportQuery>
{
    public GetAttendancePercentageReportValidator()
    {
        RuleFor(x => x.startDate)
            .LessThanOrEqualTo(x => x.endDate)
            .When(x => x.startDate.HasValue && x.endDate.HasValue)
            .WithMessage("Start date must be less than or equal to end date.");
        RuleFor(x => x.endDate)
            .GreaterThanOrEqualTo(x => x.startDate)
            .When(x => x.startDate.HasValue && x.endDate.HasValue)
            .WithMessage("End date must be greater than or equal to start date.");
    }
}
