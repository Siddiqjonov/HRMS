namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendanceSummaryReport;

public record GetAttendanceSummaryReportQuery(
    DateOnly? startDate,
    DateOnly? endDate,
    Guid? departmentId)
    : IRequest<byte[]>;
