namespace HrManager.Application.UseCases.AttendanceManagement.GetOvertimeReport;

public record GetOvertimeReportQuery(
    DateOnly? startDate,
    DateOnly? endDate,
    Guid? departmentId,
    Guid? employeeId)
    : IRequest<byte[]>;
