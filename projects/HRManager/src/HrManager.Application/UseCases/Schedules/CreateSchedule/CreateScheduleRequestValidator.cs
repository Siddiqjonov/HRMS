namespace HrManager.Application.UseCases.Schedules.CreateSchedule;

public class CreateScheduleRequestValidator : AbstractValidator<CreateScheduleRequest>
{
    public CreateScheduleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.StartTime)
            .NotEqual(x => x.EndTime)
            .WithMessage("Start time should not be equal to End time");

        RuleFor(x => x.DaysOfWeek)
            .IsInEnum().WithMessage("Invalid day of week selection.");
    }
}
