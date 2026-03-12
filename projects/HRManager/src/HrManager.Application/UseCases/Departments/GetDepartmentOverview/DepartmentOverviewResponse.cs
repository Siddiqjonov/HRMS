namespace HrManager.Application.UseCases.Departments.GetDepartmentOverview;

public record DepartmentOverviewResponse(
    Guid id,
    string name,
    string description,
    int totalEmployees,
    int presentToday,
    int absentToday,
    int lateToday,
    int onLeaveToday,
    List<TeamMemberAttendanceInfo> teamMembers,
    List<LeaveCalendarItem> upcomingLeaves);

public record TeamMemberAttendanceInfo(
    Guid id,
    string name,
    string status, // "present", "absent", "late", "on-leave"
    string? checkInTime,
    string? checkOutTime);

public record LeaveCalendarItem(
    Guid id,
    string employeeName,
    string leaveType,
    DateOnly startDate,
    DateOnly endDate,
    string status);
