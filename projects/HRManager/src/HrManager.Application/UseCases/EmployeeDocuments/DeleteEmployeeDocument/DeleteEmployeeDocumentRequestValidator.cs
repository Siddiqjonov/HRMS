namespace HrManager.Application.UseCases.EmployeeDocuments.DeleteEmployeeDocument
{
    public class DeleteEmployeeDocumentRequestValidator : AbstractValidator<DeleteEmployeeDocumentRequest>
    {
        public DeleteEmployeeDocumentRequestValidator()
        {
            RuleFor(d => d.documentId)
                .NotEmpty().WithMessage("Employee Id is required");
        }
    }
}
