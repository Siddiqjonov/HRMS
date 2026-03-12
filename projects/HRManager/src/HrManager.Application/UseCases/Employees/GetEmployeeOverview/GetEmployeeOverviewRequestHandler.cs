using HrManager.Application.Common.Services;
using HrManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.GetEmployeeOverview;

public class GetEmployeeOverviewRequestHandler(
    IApplicationDbContext context,
    IDateTimeService dateTimeService)
    : IRequestHandler<GetEmployeeOverviewRequest, EmployeeOverviewResponse>
{
    public async Task<EmployeeOverviewResponse> Handle(GetEmployeeOverviewRequest request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(dateTimeService.UtcNow);

        // Get all employees with their departments
        var employees = await context.Employees
            .Include(e => e.Department)
            .Where(e => !e.IsDeleted)
            .ToListAsync(cancellationToken);

        var employeeIds = employees.Select(e => e.Id).ToList();

        // ===== EMPLOYEE STATISTICS =====
        
        var totalEmployees = employees.Count;
        // Active employees: employees list is already filtered by !IsDeleted, 
        // so we just need to check for no termination date or future termination date
        var activeEmployees = employees.Count(e => 
            !e.TerminationDate.HasValue || e.TerminationDate.Value > today);
        
        // New hires this month
        var newHiresThisMonth = employees.Count(e => 
            e.HireDate.Year == today.Year && 
            e.HireDate.Month == today.Month);
        
        // Average tenure in years
        var averageTenure = 0.0;
        if (employees.Any())
        {
            var totalDays = employees.Sum(e => (today.ToDateTime(TimeOnly.MinValue) - e.HireDate.ToDateTime(TimeOnly.MinValue)).TotalDays);
            averageTenure = Math.Round(totalDays / employees.Count / 365.25, 1);
        }
        
        // Turnover rate (employees who left this year / average employee count)
        // For simplicity, we'll calculate based on soft-deleted employees this year
        var employeesLeftThisYear = await context.Employees
            .IgnoreQueryFilters()
            .Where(e => e.IsDeleted && 
                        e.DeletedOnUtc.HasValue && 
                        e.DeletedOnUtc.Value.Year == today.Year)
            .CountAsync(cancellationToken);
        
        var turnoverRate = totalEmployees > 0 
            ? Math.Round((double)employeesLeftThisYear / totalEmployees * 100, 1)
            : 0.0;

        var statistics = new EmployeeStatistics(
            totalEmployees,
            activeEmployees,
            newHiresThisMonth,
            averageTenure,
            turnoverRate);

        // ===== COMPANY ATTENDANCE =====
        
        // Get today's attendance records
        var todayAttendance = await context.AttendanceRecords
            .Where(a => employeeIds.Contains(a.EmployeeId) && a.Date == today)
            .ToListAsync(cancellationToken);

        // Get approved leaves for today
        var onLeaveToday = await context.AbsenceRequests
            .Where(ar => employeeIds.Contains(ar.EmployeeId) &&
                         ar.RequestStatus == RequestStatus.Approved &&
                         ar.StartDate <= today &&
                         ar.EndDate >= today &&
                         ar.RequestType != RequestType.Remote)
            .Select(ar => ar.EmployeeId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Get remote work for today
        var remoteWorkingToday = await context.AbsenceRequests
            .Where(ar => employeeIds.Contains(ar.EmployeeId) &&
                         ar.RequestStatus == RequestStatus.Approved &&
                         ar.StartDate <= today &&
                         ar.EndDate >= today &&
                         ar.RequestType == RequestType.Remote)
            .Select(ar => ar.EmployeeId)
            .Distinct()
            .CountAsync(cancellationToken);

        var present = todayAttendance.Count;
        var late = todayAttendance.Count(a => a.IsLate);
        // Absent = Total - (Present + OnLeave + RemoteWorking)
        var absent = totalEmployees - present - onLeaveToday - remoteWorkingToday;

        var attendance = new CompanyAttendance(
            totalEmployees,
            present,
            absent,
            onLeaveToday,
            late,
            remoteWorkingToday);

        // ===== UPCOMING BIRTHDAYS =====
        
        // Get birthdays in the next 30 days
        var upcomingBirthdays = new List<UpcomingBirthday>();
        
        foreach (var employee in employees.OrderBy(e => e.DateOfBirth.Month).ThenBy(e => e.DateOfBirth.Day))
        {
            // Calculate the next birthday
            var nextBirthday = new DateOnly(today.Year, employee.DateOfBirth.Month, employee.DateOfBirth.Day);
            
            // If birthday already passed this year, get next year's birthday
            if (nextBirthday < today)
            {
                nextBirthday = new DateOnly(today.Year + 1, employee.DateOfBirth.Month, employee.DateOfBirth.Day);
            }
            
            // Check if birthday is within next 30 days
            var daysUntilBirthday = (nextBirthday.ToDateTime(TimeOnly.MinValue) - today.ToDateTime(TimeOnly.MinValue)).TotalDays;
            
            if (daysUntilBirthday <= 30)
            {
                // Calculate age on that birthday
                var age = nextBirthday.Year - employee.DateOfBirth.Year;
                
                upcomingBirthdays.Add(new UpcomingBirthday(
                    employee.Id,
                    $"{employee.FirstName} {employee.LastName}",
                    employee.Department?.Name ?? "Unknown",
                    nextBirthday,
                    age));
            }
        }

        // Sort by date
        upcomingBirthdays = upcomingBirthdays
            .OrderBy(b => b.date)
            .ToList();

        return new EmployeeOverviewResponse(
            statistics,
            attendance,
            upcomingBirthdays);
    }
}
