using HrManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.Correction;

public class CorrectAttendanceRecordRequestHandler(
    IApplicationDbContext context) : IRequestHandler<CorrectAttendanceRecordRequest, bool>
{
    public async Task<bool> Handle(CorrectAttendanceRecordRequest request, CancellationToken cancellationToken)
    {
        var attendanceRecord = await context.AttendanceRecords.Include(a => a.Employee)
                .FirstOrDefaultAsync(r => r.Id == request.attendanceRecordId, cancellationToken)
                ?? throw new NotFoundException("Attendance record not found.");

        var schedule = await context.Schedules.FirstOrDefaultAsync(s => s.Id == attendanceRecord.Employee.ScheduleId, cancellationToken);

        var workMinutes = (int)(schedule!.EndTime.ToTimeSpan() - schedule.StartTime.ToTimeSpan()).TotalMinutes;

        var workedMinutes = 0;
        if (request.checkOut.HasValue)
        {
            workedMinutes = (int)((request.checkOut ?? attendanceRecord.CheckOut) !.Value.ToTimeSpan()
            - (request.checkIn ?? attendanceRecord.CheckIn).ToTimeSpan()).TotalMinutes;
        }

        var correctedValues = (
            CheckIn: request.checkIn ?? attendanceRecord.CheckIn,
            CheckOut: request.checkOut ?? attendanceRecord.CheckOut,
            IsLate: request.checkIn.HasValue ? request.checkIn > schedule!.StartTime : attendanceRecord.IsLate,
            IsEarlyDeparture: request.checkOut.HasValue ? request.checkOut < schedule!.EndTime : attendanceRecord.IsEarlyDeparture,
            TotalHours: workedMinutes,
            OvertimeHours: (workedMinutes - workMinutes) < 0 ? 0 : (workedMinutes - workMinutes));

        attendanceRecord.UpdateAttendance(correctedValues);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
