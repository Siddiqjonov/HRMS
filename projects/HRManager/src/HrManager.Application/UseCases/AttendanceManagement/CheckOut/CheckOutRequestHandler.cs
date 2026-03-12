using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.CheckOut;

public class CheckOutRequestHandler(
    IApplicationDbContext context,
    IDateTimeService dateTimeService)
    : IRequestHandler<CheckOutRequest>
{
    public async Task Handle(CheckOutRequest request, CancellationToken cancellationToken)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(x => x.Id == request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException("Employee not found.");

        var schedule = await context.Schedules.FirstOrDefaultAsync(x => x.Id == employee.ScheduleId, cancellationToken)
            ?? throw new NotFoundException("Employee schedule not assigned.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existingRecord = await context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.Date == today, cancellationToken)
            ?? throw new ConflictException("Employee has not checked in today.");

        if (existingRecord.CheckOut != default)
        {
            throw new ConflictException("Employee has already checked out today.");
        }

        var checkOutTime = TimeOnly.FromDateTime(dateTimeService.UtcNow);
        var isEarlyDeparture = checkOutTime < schedule.EndTime;
        var overtimeMinuts = (checkOutTime.ToTimeSpan() - existingRecord.CheckIn.ToTimeSpan())
            - (schedule.EndTime.ToTimeSpan() - schedule.StartTime.ToTimeSpan());
        existingRecord.CheckOutEmployee(checkOutTime, isEarlyDeparture, (int)(overtimeMinuts.TotalMinutes < 0 ? 0 : overtimeMinuts.TotalMinutes));

        await context.SaveChangesAsync(cancellationToken);
    }
}
