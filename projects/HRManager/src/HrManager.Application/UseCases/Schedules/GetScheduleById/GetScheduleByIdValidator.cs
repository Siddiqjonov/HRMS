namespace HrManager.Application.UseCases.Schedules.GetScheduleById;

public class GetScheduleByIdValidator : AbstractValidator<GetScheduleByIdRequest>
{
    public GetScheduleByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}