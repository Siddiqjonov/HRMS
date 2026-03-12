using HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.GetOvertimeReport;

public class GetOvertimeReportQueryHandler(
    IApplicationDbContext context,
    IReportService reportService)
    : IRequestHandler<GetOvertimeReportQuery, byte[]>
{
    public async Task<byte[]> Handle(GetOvertimeReportQuery request, CancellationToken cancellationToken)
    {
        var lateRecords = await context.AttendanceRecords
           .AsNoTracking()
           .Where(ar => ar.OvertimeMinutes > 0)
           .Where(ar => !request.startDate.HasValue || ar.Date >= request.startDate.Value)
           .Where(ar => !request.endDate.HasValue || ar.Date <= request.endDate.Value)
           .Where(ar => !request.departmentId.HasValue || ar.Employee.DepartmentId == request.departmentId.Value)
           .Where(ar => !request.employeeId.HasValue || ar.EmployeeId == request.employeeId)
           .Select(ar => new OvertimeReportDto
           {
               EmployeeId = ar.EmployeeId,
               EmployeeFullName = (ar.Employee.FirstName ?? string.Empty) + " " +
                                   (ar.Employee.LastName ?? string.Empty) + " " +
                                   (ar.Employee.MiddleName ?? string.Empty),
               DepartmentName = ar.Employee.Department != null ? ar.Employee.Department.Name : string.Empty,
               Date = ar.Date,
               OvertimeMinutes = ar.OvertimeMinutes,
               OvertimeCost = ar.OvertimeMinutes * ar.Employee.Salary / (decimal)(22 * 8 * 60),
           })
           .OrderBy(r => r.EmployeeFullName)
           .ThenBy(r => r.DepartmentName)
           .ThenBy(r => r.Date)
           .ToListAsync(cancellationToken);

        var reportBytes = await reportService.ExportOvertimeReportAsync(lateRecords);
        return reportBytes;
    }
}
