namespace HrManager.Application.UseCases.Employees.GetEmployeeOverview;

public record EmployeeOverviewResponse(
    EmployeeStatistics statistics,
    CompanyAttendance attendance,
    List<UpcomingBirthday> upcomingBirthdays);

public record EmployeeStatistics(
    int totalEmployees,
    int activeEmployees,
    int newHiresThisMonth,
    double averageTenure,
    double turnoverRate);

public record CompanyAttendance(
    int totalEmployees,
    int present,
    int absent,
    int onLeave,
    int late,
    int remoteWorking);

public record UpcomingBirthday(
    Guid id,
    string employeeName,
    string department,
    DateOnly date,
    int age);
