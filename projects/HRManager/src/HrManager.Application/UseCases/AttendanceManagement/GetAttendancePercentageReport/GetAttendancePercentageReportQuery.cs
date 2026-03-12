namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendancePercentageReport;

public record GetAttendancePercentageReportQuery(
    DateOnly? startDate,
    DateOnly? endDate,
    Guid? departmentId)
    : IRequest<byte[]>;
