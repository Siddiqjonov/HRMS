using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendanceRecordsWithFilter;

public record GetAttendanceRecordsRequest(
     Guid? employeeId,
     DateOnly? startDate,
     DateOnly? endDate,
     bool? isLate,
     bool? isEarlyDeparture,
     int pageNumber = 1,
     int pageSize = 10)
    : IRequest<PaginatedList<GetAttendanceRecordsResponse>>;
