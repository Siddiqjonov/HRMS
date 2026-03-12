using HrManager.Domain.Enums;

namespace HrManager.Application.Common.Interfaces;

public interface IAbsenceBalanceService
{
    Task<bool> HasSufficientBalanceAsync(Guid employeeId, RequestType type, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    Task<int> GetRemainingDaysAsync(Guid employeeId, RequestType type, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingRequestAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    Task DeductBalanceAsync(Guid employeeId, RequestType type, DateOnly startDate, DateOnly endDate,  CancellationToken cancellationToken = default);

    int CalculateRequestedDays(DateOnly start, DateOnly end, RequestType type);
}
