namespace HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;

public class AttendanceReportDto
{
    public Guid EmployeeId { get; set; }

    public string? EmployeeFullName { get; set; }

    public string? DepartmentName { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly CheckIn { get; set; }

    public TimeOnly? CheckOut { get; set; }

    public int TotalMinutes { get; set; }

    public bool IsLate { get; set; }

    public bool IsEarlyDeparture { get; set; }
}
