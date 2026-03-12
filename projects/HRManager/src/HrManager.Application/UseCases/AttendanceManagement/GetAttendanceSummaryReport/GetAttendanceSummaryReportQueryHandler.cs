using HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendanceSummaryReport;

public class GetAttendanceSummaryReportQueryHandler(
    IApplicationDbContext context,
    IReportService reportService)
    : IRequestHandler<GetAttendanceSummaryReportQuery, byte[]>
{
    public async Task<byte[]> Handle(GetAttendanceSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var query = context.AttendanceRecords
            .Include(ar => ar.Employee).ThenInclude(e => e.Department).AsNoTracking();

        query = query.Where(ar => !request.startDate.HasValue || ar.Date >= request.startDate.Value);
        query = query.Where(ar => !request.endDate.HasValue || ar.Date <= request.endDate.Value);
        query = query.Where(ar => !request.departmentId.HasValue || ar.Employee.DepartmentId == request.departmentId.Value);

        var data = await query
            .GroupBy(ar => new
            {
                ar.EmployeeId,
                EmployeeFullName = (ar.Employee.FirstName ?? string.Empty) + " " + 
                                   (ar.Employee.LastName ?? string.Empty) + " " + 
                                   (ar.Employee.MiddleName ?? string.Empty),
                DepartmentName = ar.Employee.Department != null ? ar.Employee.Department.Name : string.Empty,
            })
            .Select(g => new AttendanceSummaryReportDto
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeFullName = g.Key.EmployeeFullName,
                DepartmentName = g.Key.DepartmentName,
                TotalDaysWorked = g.Count(),
                TotalLateArrivals = g.Count(ar => ar.IsLate),
                TotalEarlyDepartures = g.Count(ar => ar.IsEarlyDeparture),
                TotalWorkMinutes = g.Sum(ar => ar.TotalMinutes),
                TotalOvertimeMinutes = g.Sum(ar => ar.OvertimeMinutes),
            })
            .OrderBy(dto => dto.DepartmentName)
            .ThenBy(dto => dto.EmployeeFullName)
            .ToListAsync(cancellationToken);

        return await reportService.ExportAttendanceSummaryAsync(data, request.startDate, request.endDate ?? DateOnly.FromDateTime(DateTime.Now));
    }
}
