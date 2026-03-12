namespace HrManager.Application.UseCases.Positions.GetPositionsByDeparmentId;

public class GetPositionsByDepartmentIdValidator : AbstractValidator<GetPositionsByDepartmentIdRequest>
{
    public GetPositionsByDepartmentIdValidator()
    {
        RuleFor(d => d.DepartmentId)
             .NotEmpty().WithMessage("DepartmentId is required.");
    }
}
