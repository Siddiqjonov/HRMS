using HrManager.Api.Infrastructure;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.AttendanceManagement.GetAttendancePercentageReport;
using HrManager.Application.UseCases.AttendanceManagement.GetAttendanceSummaryReport;
using HrManager.Application.UseCases.AttendanceManagement.GetLateArrivalReport;
using HrManager.Application.UseCases.AttendanceManagement.GetOvertimeReport;
using HrManager.Domain.Constants;
using MediatR;

namespace HrManager.Api.Endpoints;

public class Reports : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization(Policies.RequireEmployeeOrHrManager);
        groupBuilder.MapGet(ExportEmployeeList, "/ExportEmployeeList");
        groupBuilder.MapGet(DepartmentSummary, "/Departments/Summary");
        groupBuilder.MapGet(GetAttendanceSummaryReport, "/Attendance/Summary");
        groupBuilder.MapGet(GetLateArrivalReport, "/Attendance/LateArrivals");
        groupBuilder.MapGet(GetOverTimeReport, "/Attendance/Overtime");
        groupBuilder.MapGet(GetAttendancePercentageReport, "/Attendance/Percentage");
    }

    public static async Task<IResult> ExportEmployeeList(
         IReportService reportService,
         Guid? departmentId,
         DateOnly? startDate,
         DateOnly? endDate)
    {
        var result = await reportService.ExportEmployeeListAsync(departmentId, startDate, endDate);

        return Results.File(
           result,
           contentType: "application/vnd.ms-excel",
           fileDownloadName: $"EmployeeList_{DateTime.Now:yyyyMMddHHmm}.xlsx"
       );
    }

    public static async Task<IResult> DepartmentSummary(IReportService reportService, Guid? departmentId)
    {
        var result = await reportService.GenerateDepartmentSummaryReportAsync(departmentId);

        return Results.File(
            result,
            contentType: "application/vnd.ms-excel",
            fileDownloadName: $"DepartmentSummary_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        );
    }

    public static async Task<IResult> GetAttendancePercentageReport(
        ISender sender,
        DateOnly? startDate,
        DateOnly? endDate,
        Guid? departmentId)
    {
        var query = new GetAttendancePercentageReportQuery(startDate, endDate, departmentId);
        var result = await sender.Send(query);
        return Results.File(
            result,
            contentType: "application/vnd.ms-excel",
            fileDownloadName: $"AttendancePercentageReport_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        );
    }

    public static async Task<IResult> GetOverTimeReport(
        ISender sender,
        DateOnly? startDate,
        DateOnly? endDate,
        Guid? departmentId,
        Guid? employeeId)
    {
        var query = new GetOvertimeReportQuery(startDate, endDate, departmentId, employeeId);
        var result = await sender.Send(query);
        return Results.File(
            result,
            contentType: "application/vnd.ms-excel",
            fileDownloadName: $"OvertimeReport_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        );
    }

    public static async Task<IResult> GetAttendanceSummaryReport(
        ISender sender,
        DateOnly? startDate,
        DateOnly? endDate,
        Guid? departmentId)
    {
        var query = new GetAttendanceSummaryReportQuery(startDate, endDate, departmentId);
        var result = await sender.Send(query);
        return Results.File(
            result,
            contentType: "application/vnd.ms-excel",
            fileDownloadName: $"AttendanceSummary_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        );
    }

    public static async Task<IResult> GetLateArrivalReport(
    ISender sender,
    DateOnly? startDate,
    DateOnly? endDate,
    Guid? departmentId,
    Guid? employeeId)
    {
        var query = new GetLateArrivalReportQuery(startDate, endDate, departmentId, employeeId);
        var result = await sender.Send(query);
        return Results.File(
            result,
            contentType: "application/vnd.ms-excel",
            fileDownloadName: $"LateArrivalReport_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        );
    }
}
