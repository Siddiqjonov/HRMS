using HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AttendanceManagement.GetLateArrivalReport;

public class GetLateArrivalReportQueryHandler(
    IApplicationDbContext context,
    IReportService reportService)
    : IRequestHandler<GetLateArrivalReportQuery, byte[]>
{
    public async Task<byte[]> Handle(GetLateArrivalReportQuery request, CancellationToken cancellationToken)
    {
        var lateRecords = await context.AttendanceRecords
            .AsNoTracking()
            .Where(ar => ar.CheckIn > ar.Employee.WorkSchedule.StartTime)
            .Where(ar => !request.startDate.HasValue || ar.Date >= request.startDate.Value)
            .Where(ar => !request.endDate.HasValue || ar.Date <= request.endDate.Value)
            .Where(ar => !request.departmentId.HasValue || ar.Employee.DepartmentId == request.departmentId.Value)
            .Where(ar => !request.employeeId.HasValue || ar.Employee.Id == request.employeeId)
            .Select(ar => new LateArrivalReportDto
            {
                EmployeeId = ar.EmployeeId,
                EmployeeFullName = (ar.Employee.FirstName ?? string.Empty) + " " +
                                   (ar.Employee.LastName ?? string.Empty) + " " +
                                   (ar.Employee.MiddleName ?? string.Empty),
                DepartmentName = ar.Employee.Department != null ? ar.Employee.Department.Name : string.Empty,
                Date = ar.Date,
                CheckIn = ar.CheckIn,
                LateMinutes = (int)(ar.CheckIn - ar.Employee.WorkSchedule.StartTime).TotalMinutes,
            })
            .OrderBy(r => r.EmployeeFullName)
            .ThenBy(r => r.DepartmentName)
            .ThenBy(r => r.Date)
            .ToListAsync(cancellationToken);
        
        return await reportService.ExportLateArrivalReportAsync(lateRecords);
    }
}
