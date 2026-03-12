namespace HrManager.Application.UseCases.Positions.GetPositionsWithPagination;

public class GetPositionsWithPaginationRequestValidator : AbstractValidator<GetPositionsWithPaginationRequest>
{
    public GetPositionsWithPaginationRequestValidator()
    {
        RuleFor(x => x.pageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.pageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
