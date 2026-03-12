using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendanceRecordsWithFilter;

public class GetAttendanceRecordsRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetAttendanceRecordsRequest, PaginatedList<GetAttendanceRecordsResponse>>
{
    public async Task<PaginatedList<GetAttendanceRecordsResponse>> Handle(GetAttendanceRecordsRequest request, CancellationToken cancellationToken)
    {
        var query = context.AttendanceRecords.AsNoTracking()
            .Where(ar => !request.employeeId.HasValue || ar.EmployeeId == request.employeeId)
            .Where(ar => !request.startDate.HasValue || ar.Date >= request.startDate)
            .Where(ar => !request.endDate.HasValue || ar.Date <= request.endDate)
            .Where(ar => !request.isLate.HasValue || ar.IsLate == request.isLate)
            .Where(ar => !request.isEarlyDeparture.HasValue || ar.IsEarlyDeparture == request.isEarlyDeparture)
            .Select(ar => new GetAttendanceRecordsResponse(
                ar.Id,
                ar.EmployeeId,
                (ar.Employee.FirstName ?? string.Empty) + " " + (ar.Employee.LastName ?? string.Empty),
                ar.Date,
                ar.CheckIn,
                ar.CheckOut,
                ar.OvertimeMinutes,
                ar.TotalMinutes,
                ar.IsLate,
                ar.IsEarlyDeparture));

        return await PaginatedList<GetAttendanceRecordsResponse>
            .CreateAsync(query, request.pageNumber, request.pageSize, cancellationToken);
    }
}
