namespace HrManager.Application.UseCases.Positions.GetPositionById;

public class GetPositionByIdValidator : AbstractValidator<GetPositionById>
{
    public GetPositionByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
