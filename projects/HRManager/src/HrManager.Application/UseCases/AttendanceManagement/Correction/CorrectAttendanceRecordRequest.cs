namespace HrManager.Application.UseCases.AttendanceManagement.Correction;

public record CorrectAttendanceRecordRequest(
    Guid attendanceRecordId,
    TimeOnly? checkIn = null,
    TimeOnly? checkOut = null)
    : IRequest<bool>;
