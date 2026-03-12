namespace HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;

public class OvertimeReportDto
{
    public Guid EmployeeId { get; set; }

    public string? EmployeeFullName { get; set; }

    public string? DepartmentName { get; set; }

    public DateOnly Date { get; set; }

    public int OvertimeMinutes { get; set; }

    public decimal OvertimeCost { get; set; }
}
