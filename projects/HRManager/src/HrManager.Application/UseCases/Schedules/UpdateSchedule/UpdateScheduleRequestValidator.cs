namespace HrManager.Application.UseCases.Schedules.UpdateSchedule;

public class UpdateScheduleRequestValidator : AbstractValidator<UpdateScheduleRequest>
{
    public UpdateScheduleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description must not exceed 250 characters.");

        RuleFor(x => x.StartTime)
            .NotEqual(x => x.EndTime)
            .WithMessage("Start time should not be equal to End time");

        RuleFor(x => x.DaysOfWeek)
            .IsInEnum()
            .WithMessage("Invalid day(s) of week value.");
    }
}
