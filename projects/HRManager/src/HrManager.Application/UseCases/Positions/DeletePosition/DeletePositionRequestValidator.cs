namespace HrManager.Application.UseCases.Positions.DeletePosition;

public class DeletePositionRequestValidator : AbstractValidator<DeletePositionRequest>
{
    public DeletePositionRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
