using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using HrManager.Application.Common.Services.EmailService.Templates;
using HrManager.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.CheckIn;

public class CheckInRequestHandler(
    IApplicationDbContext context,
    IEmailService emailService,
    IDateTimeService dateTimeService) : IRequestHandler<CheckInRequest>
{
    public async Task Handle(CheckInRequest request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(dateTimeService.UtcNow);

        var existingRecord = await context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.Date == today, cancellationToken);

        if (existingRecord is not null)
        {
            throw new ConflictException("Employee has already checked in today.");
        }

        var employee = await context.Employees.FirstOrDefaultAsync(x => x.Id == request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException("Employee not found.");

        var schedule = await context.Schedules.FirstOrDefaultAsync(x => x.Id == employee.ScheduleId, cancellationToken)
            ?? throw new NotFoundException("Employee schedule not assigned.");

        var checkInTime = TimeOnly.FromDateTime(dateTimeService.UtcNow);
        bool isLate = checkInTime > schedule.StartTime;

        var attendanceRecordDto = new AttendanceRecordDto
        {
            EmployeeId = request.EmployeeId,
            Date = today,
            CheckIn = checkInTime,
            IsLate = isLate,
            IsEarlyDeparture = false,
            TotalMinutes = default,
            OvertimeMinutes = default,
        };

        var attendanceRecord = new AttendanceRecord(attendanceRecordDto);

        await context.AttendanceRecords.AddAsync(attendanceRecord, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var consecutiveLates = context.AttendanceRecords
            .Where(a => a.EmployeeId == request.EmployeeId)
            .OrderByDescending(a => a.Date)
            .AsEnumerable()
            .TakeWhile(a => a.IsLate)
            .ToList();

        if (consecutiveLates.Count > 1)
        {
            var template = EmailTemplates.ConsecutiveLateArrivalWarning(
                employee.FirstName, 
                consecutiveLates.Count, 
                new List<string> { employee!.Email });

            await emailService.SendEmailTemplateAsync(template, cancellationToken);
        }
    }
}
