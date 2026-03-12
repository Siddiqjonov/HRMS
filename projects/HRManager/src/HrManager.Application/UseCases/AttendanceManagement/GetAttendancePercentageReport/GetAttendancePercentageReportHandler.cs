using HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.GetAttendancePercentageReport;

public class GetAttendancePercentageReportHandler(
    IApplicationDbContext context,
    IReportService reportService)
    : IRequestHandler<GetAttendancePercentageReportQuery, byte[]>
{
    public async Task<byte[]> Handle(GetAttendancePercentageReportQuery request, CancellationToken cancellationToken)
    {
        var attendanceData = await context.AttendanceRecords
       .AsNoTracking().Include(ar => ar.Employee).ThenInclude(e => e.Department)
       .Where(ar => !request.startDate.HasValue || ar.Employee.HireDate >= request.startDate.Value)
       .Where(ar => !request.endDate.HasValue || ar.Employee.TerminationDate.HasValue ? ar.Employee.TerminationDate <= request.endDate : DateOnly.FromDateTime(DateTime.Now) <= request.endDate.Value)
       .Where(ar => !request.departmentId.HasValue || ar.Employee.DepartmentId == request.departmentId.Value)
       .Select(ar => new AttendancePercentageReportDto
       {
           EmployeeId = ar.EmployeeId,
           EmployeeFullName = (ar.Employee.FirstName ?? string.Empty) + " " +
                                   (ar.Employee.LastName ?? string.Empty) + " " +
                                   (ar.Employee.MiddleName ?? string.Empty),
           DepartmentName = ar.Employee.Department != null ? ar.Employee.Department.Name : string.Empty,
           PeriodStartDate = ar.Employee.HireDate,
           PeriodEndDate = ar.Employee.TerminationDate ?? DateOnly.FromDateTime(DateTime.Now),
       })
       .OrderBy(r => r.EmployeeFullName)
       .ThenBy(r => r.DepartmentName)
       .ThenBy(r => r.PeriodStartDate)
       .ToListAsync(cancellationToken);

        var reportBytes = await reportService.ExportAttendancePercentageReportAsync(attendanceData);
        return reportBytes;
    }
}
