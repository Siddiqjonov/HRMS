namespace HrManager.Application.UseCases.Schedules.GetScheduleEmployees;

public class GetScheduleEmployeesRequestValidator : AbstractValidator<GetScheduleEmployeesRequest>
{
    public GetScheduleEmployeesRequestValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("ScheduleId is required.");
    }
}
