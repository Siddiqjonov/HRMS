namespace HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;

public class DepartmentAttendanceReportDto
{
    public Guid DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public double AverageAttendancePercentage { get; set; }

    public int TotalEmployees { get; set; }
}
