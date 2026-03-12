namespace HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;

public class AttendanceSummaryReportDto
{
    public Guid EmployeeId { get; set; }

    public string? EmployeeFullName { get; set; }

    public string? DepartmentName { get; set; }
    
    public int TotalDaysWorked { get; set; }
    
    public int TotalLateArrivals { get; set; }
    
    public int TotalEarlyDepartures { get; set; }
    
    public int TotalWorkMinutes { get; set; }
    
    public int TotalOvertimeMinutes { get; set; }
}
