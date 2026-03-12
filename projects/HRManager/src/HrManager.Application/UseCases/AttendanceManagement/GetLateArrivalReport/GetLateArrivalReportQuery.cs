namespace HrManager.Application.UseCases.AttendanceManagement.GetLateArrivalReport;

public record GetLateArrivalReportQuery(
    DateOnly? startDate,
    DateOnly? endDate,
    Guid? departmentId,
    Guid? employeeId)
    : IRequest<byte[]>;
