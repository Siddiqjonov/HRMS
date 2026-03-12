namespace HrManager.Domain.Dtos;

public class AttendanceRecordDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly CheckIn { get; set; }

    public TimeOnly? CheckOut { get; set; }

    public int TotalMinutes { get; set; }

    public int OvertimeMinutes { get; set; }

    public bool IsLate { get; set; }

    public bool IsEarlyDeparture { get; set; }
}
