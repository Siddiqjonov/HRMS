namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestResponses;

public class ProcessAbsenceRequestValidator : AbstractValidator<ProcessAbsenceRequest>
{
    public ProcessAbsenceRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Absence request Id is required.");
    }
}
