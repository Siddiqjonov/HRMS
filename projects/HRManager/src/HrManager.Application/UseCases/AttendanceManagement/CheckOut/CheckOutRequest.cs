namespace HrManager.Application.UseCases.AttendanceManagement.CheckOut;

public record CheckOutRequest(Guid EmployeeId) : IRequest;
