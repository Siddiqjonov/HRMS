namespace HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;

public class AttendancePercentageReportDto
{
    public Guid EmployeeId { get; set; }

    public string? EmployeeFullName { get; set; }

    public string? DepartmentName { get; set; }

    public double AttendancePercentage { get; set; }

    public DateOnly PeriodStartDate { get; set; }

    public DateOnly PeriodEndDate { get; set; }
}
