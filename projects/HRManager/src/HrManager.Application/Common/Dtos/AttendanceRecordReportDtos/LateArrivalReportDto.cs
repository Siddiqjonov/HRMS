namespace HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;

public class LateArrivalReportDto
{
    public Guid EmployeeId { get; set; }

    public string? EmployeeFullName { get; set; }

    public string? DepartmentName { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly CheckIn { get; set; }

    public int LateMinutes { get; set; }
}
