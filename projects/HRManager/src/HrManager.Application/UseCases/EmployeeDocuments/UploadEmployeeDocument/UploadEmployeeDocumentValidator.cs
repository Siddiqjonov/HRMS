namespace HrManager.Application.UseCases.EmployeeDocuments.UploadEmployeeDocument;

public class UploadEmployeeDocumentValidator : AbstractValidator<UploadEmployeeDocumentRequest>
{
    private static readonly HashSet<string> AllowedTypes = new (StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "image/jpeg",
        "image/png",
    };

    private const long MaxFileSize = 10 * 1024 * 1024;

    public UploadEmployeeDocumentValidator()
    {
        RuleFor(x => x.employeeId).NotEmpty();
        RuleFor(x => x.file).NotNull();
        RuleFor(x => x.file.FileName).NotEmpty();
        RuleFor(x => x.file.Length)
            .LessThanOrEqualTo(MaxFileSize).WithMessage("File too large (max 10MB)");
        RuleFor(x => x.file.ContentType)
            .Must(ct => AllowedTypes.Contains(ct))
            .WithMessage("Invalid file type");
        RuleFor(x => x.documentType).IsInEnum();
    }
}
