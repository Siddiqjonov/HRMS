using HrManager.Application.Common.Dtos.AttendanceRecordReportDtos;
using OfficeOpenXml;

namespace HrManager.Application.Common.Interfaces;

public interface IReportService
{
    Task<byte[]> ExportEmployeeListAsync(Guid? departmentId = null, DateOnly? startDate = null, DateOnly? endDate = null);

    Task<byte[]> GenerateDepartmentSummaryReportAsync(Guid? departmentId = null);

    Task<ExcelWorksheet> CreateReportTemplateAsync(ExcelPackage package, string title, int columnCount);

    Task<byte[]> ExportAttendanceSummaryAsync(List<AttendanceSummaryReportDto> data, DateOnly? startDate, DateOnly endDate);

    Task<byte[]> ExportLateArrivalReportAsync(List<LateArrivalReportDto> data);

    Task<byte[]> ExportOvertimeReportAsync(List<OvertimeReportDto> data);

    Task<byte[]> ExportAttendancePercentageReportAsync(List<AttendancePercentageReportDto> data);
}
