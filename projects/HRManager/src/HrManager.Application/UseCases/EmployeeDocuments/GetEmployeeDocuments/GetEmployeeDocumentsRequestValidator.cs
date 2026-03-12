namespace HrManager.Application.UseCases.EmployeeDocuments.GetEmployeeDocuments;

public class GetEmployeeDocumentsRequestValidator : AbstractValidator<GetEmployeeDocumentsRequest>
{
    public GetEmployeeDocumentsRequestValidator()
    {
        RuleFor(x => x.pageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.pageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.documentType)
            .IsInEnum().When(x => x.documentType.HasValue);
    }
}
