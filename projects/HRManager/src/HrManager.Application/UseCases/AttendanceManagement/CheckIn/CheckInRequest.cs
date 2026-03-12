namespace HrManager.Application.UseCases.AttendanceManagement.CheckIn;

public record CheckInRequest(Guid EmployeeId) : IRequest;
