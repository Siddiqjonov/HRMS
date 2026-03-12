using HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestConfiguration;
using HrManager.Domain.Dtos;
using HrManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HrManager.Application.Common.Services;

public class AbsenceBalanceService(
    IApplicationDbContext context,
    IOptions<AbsencePoliciesConfig> policies) : IAbsenceBalanceService
{
    public int CalculateRequestedDays(DateOnly start, DateOnly end, RequestType type)
    {
        return SplitByPolicyPeriods(type, start, end)
            .Sum(period =>
            {
                var (startInPeriod, endInPeriod) = GetEffectivePeriod(period, start, end);
                return endInPeriod.DayNumber - startInPeriod.DayNumber + 1;
            });
    }

    public async Task DeductBalanceAsync(Guid employeeId, RequestType type, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        var periods = SplitByPolicyPeriods(type, startDate, endDate).ToList();

        var balances = await context.EmployeeAbsenceBalances
                .Where(b => b.EmployeeId == employeeId && b.AbsenceType == type)
                .ToListAsync(cancellationToken);

           balances = balances
                .Where(b => periods.Any(p => p.Start == b.PeriodStartDate && p.End == b.PeriodEndDate))
                .ToList();

        foreach (var period in periods)
        {
            var (startInPeriod, endInPeriod) = GetEffectivePeriod(period, startDate, endDate);
            var daysToDeduct = endInPeriod.DayNumber - startInPeriod.DayNumber + 1;

            var balance = GetOrCreateBalance(employeeId, type, period, balances);
            balance.UseDays(daysToDeduct);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetRemainingDaysAsync(Guid employeeId, RequestType type, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        var periods = SplitByPolicyPeriods(type, startDate, endDate).ToList();

        var balances = await LoadBalances(employeeId, type, periods, cancellationToken);

        return periods.Sum(period =>
        {
            var balance = balances.FirstOrDefault(b => b.PeriodStartDate == period.Start && b.PeriodEndDate == period.End);
            return balance != null ? balance.TotalDaysAllowed - balance.DaysUsed : GetPolicyDaysAllowed(type);
        });
    }

    public async Task<bool> HasOverlappingRequestAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await context.AbsenceRequests
        .AsNoTracking()
        .AnyAsync(
            r =>
                r.EmployeeId == employeeId &&
                (r.RequestStatus == RequestStatus.Pending || r.RequestStatus == RequestStatus.Approved) &&
                startDate <= r.EndDate &&
                endDate >= r.StartDate,
            cancellationToken);
    }

    public async Task<bool> HasSufficientBalanceAsync(Guid employeeId, RequestType type, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        var periods = SplitByPolicyPeriods(type, startDate, endDate).ToList();
        var balances = await LoadBalances(employeeId, type, periods, cancellationToken, asNoTracking: true);

        foreach (var period in periods)
        {
            var (startInPeriod, endInPeriod) = GetEffectivePeriod(period, startDate, endDate);
            var daysRequested = endInPeriod.DayNumber - startInPeriod.DayNumber + 1;

            var balance = balances.FirstOrDefault(b => b.PeriodStartDate == period.Start && b.PeriodEndDate == period.End);
            var remaining = balance != null ? balance.TotalDaysAllowed - balance.DaysUsed : GetPolicyDaysAllowed(type);

            if (remaining < daysRequested) return false;
        }

        return true;
    }

    private (DateOnly Start, DateOnly End) GetEffectivePeriod((DateOnly Start, DateOnly End) period, DateOnly startDate, DateOnly endDate)
    {
        var startInPeriod = startDate > period.Start ? startDate : period.Start;
        var endInPeriod = endDate < period.End ? endDate : period.End;
        return (startInPeriod, endInPeriod);
    }

    private async Task<List<EmployeeAbsenceBalance>> LoadBalances(Guid employeeId, RequestType type, IEnumerable<(DateOnly Start, DateOnly End)> periods, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        var query = context.EmployeeAbsenceBalances
        .Where(b => b.EmployeeId == employeeId && b.AbsenceType == type);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var balances = await query.ToListAsync(cancellationToken);

        return balances
              .Where(b => periods.Any(p => p.Start == b.PeriodStartDate && p.End == b.PeriodEndDate))
              .ToList();
    }

    private EmployeeAbsenceBalance GetOrCreateBalance(Guid employeeId, RequestType type, (DateOnly Start, DateOnly End) period, List<EmployeeAbsenceBalance> balances)
    {
        var balance = balances.FirstOrDefault(b => b.PeriodStartDate == period.Start && b.PeriodEndDate == period.End);
        if (balance != null)
        {
            return balance;
        }

        balance = new EmployeeAbsenceBalance(new EmployeeAbsenceBalanceDto
        {
            EmployeeId = employeeId,
            AbsenceType = type,
            TotalDaysAllowed = GetPolicyDaysAllowed(type),
            DaysUsed = 0,
            PeriodStartDate = period.Start,
            PeriodEndDate = period.End,
        });

        balances.Add(balance);
        context.EmployeeAbsenceBalances.Add(balance);
        return balance;
    }

    private int GetPolicyDaysAllowed(RequestType type)
    {
        return type switch
        {
            RequestType.Vacation => policies.Value.Vacation.DaysAllowed,
            RequestType.Remote => policies.Value.Remote.DaysAllowed,
            _ => int.MaxValue,
        };
    }

    private (DateOnly Start, DateOnly End) GetPolicyPeriod(RequestType type, DateOnly startDate)
    {
        return type switch
        {
            RequestType.Vacation => (
                new DateOnly(startDate.Year, 1, 1),
                new DateOnly(startDate.Year, 12, 31)),

            RequestType.Remote => (
                new DateOnly(startDate.Year, startDate.Month, 1),
                new DateOnly(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month))),

            _ => (startDate, startDate)
        };
    }

    private IEnumerable<(DateOnly Start, DateOnly End)> SplitByPolicyPeriods(RequestType type, DateOnly start, DateOnly end)
    {
        var periods = new List<(DateOnly, DateOnly)>();
        var current = start;

        while (current <= end)
        {
            var (periodStart, periodEnd) = GetPolicyPeriod(type, current);
            periods.Add((periodStart, periodEnd));
            current = periodEnd.AddDays(1);
        }

        return periods;
    }
}
