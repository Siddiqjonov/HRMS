namespace HrManager.Application.UseCases.AbsenceRequests;

public class CreateAbsenceRequestValidator : AbstractValidator<CreateAbsenceRequestRequest>
{
    public CreateAbsenceRequestValidator(
        IAbsenceBalanceService balanceService)
    {
        RuleFor(x => x.EmployeeId).NotEmpty();

        RuleFor(x => x.StartDate)
            .Must(start => start >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be after start date.");

        RuleFor(x => x)
            .MustAsync(async (req, ct) =>
            {
                return !await balanceService.HasOverlappingRequestAsync(
                    req.EmployeeId,
                    req.StartDate,
                    req.EndDate,
                    ct);
            })
            .WithMessage("There is already an overlapping request for this period.");

        RuleFor(x => x)
            .MustAsync(async (req, ct) =>
            {
                return await balanceService.HasSufficientBalanceAsync(
                    req.EmployeeId,
                    req.RequestType,
                    req.StartDate,  
                    req.EndDate,
                    ct);
            })
            .WithMessage("Insufficient absence balance.");

    }
}
