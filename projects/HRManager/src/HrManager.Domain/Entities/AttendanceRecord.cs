using HrManager.Domain.Dtos;

namespace HrManager.Domain.Entities;

public class AttendanceRecord : SoftDeletableAuditableEntity
{
    public AttendanceRecord() { }

    public AttendanceRecord(AttendanceRecordDto dto)
    {
        EmployeeId = dto.EmployeeId;
        Date = dto.Date;
        CheckIn = dto.CheckIn;
        CheckOut = dto.CheckOut;
        TotalMinutes = dto.TotalMinutes;
        OvertimeMinutes = dto.OvertimeMinutes;
        IsLate = dto.IsLate;
        IsEarlyDeparture = dto.IsEarlyDeparture;
    }

    public Guid EmployeeId { get; private set; }

    public Employee Employee { get; set; }

    public DateOnly Date { get; private set; }

    public TimeOnly CheckIn { get; private set; }

    public TimeOnly? CheckOut { get; private set; }

    public int OvertimeMinutes { get; private set; }

    public int TotalMinutes { get; private set; }

    public bool IsLate { get; private set; }

    public bool IsEarlyDeparture { get; private set; }

    public void CheckOutEmployee(TimeOnly checkOutTime, bool isEarlyDeparture, int overtimeMinutes)
    {
        OvertimeMinutes = overtimeMinutes;
        CheckOut = checkOutTime;
        TotalMinutes = (int)(checkOutTime.ToTimeSpan() - CheckIn.ToTimeSpan()).TotalMinutes;
        IsEarlyDeparture = isEarlyDeparture;
    }

    public void UpdateAttendance((
    TimeOnly CheckIn, TimeOnly? CheckOut,
    bool IsLate, bool IsEarlyDeparture,
    int TotalMinutes, int OvertimeMinutes) correctedValues)
    {
        CheckIn = correctedValues.CheckIn;
        CheckOut = correctedValues.CheckOut;
        IsLate = correctedValues.IsLate;
        IsEarlyDeparture = correctedValues.IsEarlyDeparture;
        TotalMinutes = correctedValues.TotalMinutes;
        OvertimeMinutes = correctedValues.OvertimeMinutes;
    }
}
