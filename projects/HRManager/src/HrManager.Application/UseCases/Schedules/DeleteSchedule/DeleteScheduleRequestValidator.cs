namespace HrManager.Application.UseCases.Schedules.DeleteSchedule;

public class DeleteScheduleRequestValidator : AbstractValidator<DeleteScheduleRequest>
{
    public DeleteScheduleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
