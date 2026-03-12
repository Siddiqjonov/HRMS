namespace HrManager.Application.UseCases.AttendanceManagement.Correction;

public class CorrectAttendanceRecordRequestValidator : AbstractValidator<CorrectAttendanceRecordRequest>
{
    public CorrectAttendanceRecordRequestValidator()
    {
        RuleFor(x => x.attendanceRecordId)
                .NotEmpty().WithMessage("AttendanceRecordId is required.");

        RuleFor(x => x.checkOut)
            .Must((request, checkOut) =>
                !checkOut.HasValue || (request.checkIn.HasValue && checkOut.Value > request.checkIn.Value))
            .WithMessage("CheckOut must be after CheckIn.");

        RuleFor(x => x)
            .Must(x => x.checkIn.HasValue || x.checkOut.HasValue)
            .WithMessage("At least one of CheckIn or CheckOut must be provided.");
    }
}
