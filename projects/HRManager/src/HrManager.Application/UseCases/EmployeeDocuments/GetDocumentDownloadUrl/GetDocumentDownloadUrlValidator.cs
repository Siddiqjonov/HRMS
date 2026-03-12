namespace HrManager.Application.UseCases.EmployeeDocuments.GetDocumentDownloadUrl;

public class GetDocumentDownloadUrlValidator : AbstractValidator<GetDocumentDownloadUrlRequest>
{
    public GetDocumentDownloadUrlValidator()
    {
        RuleFor(x => x.documentId)
            .NotEmpty().WithMessage("DocumentId is required.");
    }
}
