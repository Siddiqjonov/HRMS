namespace HrManager.Application.UseCases.Positions.GetPositionEmployees
{
    public class GetPositionEmployeesRequestValidator : AbstractValidator<GetPositionEmployeesRequest>
    {
        public GetPositionEmployeesRequestValidator()
        {
            RuleFor(x => x.PositionId)
                .NotEmpty().WithMessage("PositionId is required.");
        }
    }
}
