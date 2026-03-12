namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendanceRecordsWithFilter;

public record GetAttendanceRecordsResponse(
     Guid attendanceId,
     Guid employeeId,
     string employeeName,
     DateOnly? date,
     TimeOnly? checkIn,
     TimeOnly? checkOut,
     int? overtimeMinutes,
     int? totalMinutes,
     bool? isLate,
     bool? isEarlyDeparture);
