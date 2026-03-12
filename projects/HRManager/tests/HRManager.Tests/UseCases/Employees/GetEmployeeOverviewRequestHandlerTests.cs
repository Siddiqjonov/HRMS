using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.Employees.GetEmployeeOverview;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Employees;

public class GetEmployeeOverviewRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IDateTimeService> _dateTimeServiceMock;
    private readonly GetEmployeeOverviewRequestHandler _handler;

    public GetEmployeeOverviewRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _dateTimeServiceMock = new Mock<IDateTimeService>();
        
        _handler = new GetEmployeeOverviewRequestHandler(
            _contextMock.Object, 
            _dateTimeServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmployeeOverview_WithCorrectStatistics()
    {
        // Arrange
        var today = new DateTime(2025, 12, 15);
        var todayDate = DateOnly.FromDateTime(today);
        
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(today);

        // Create test departments
        var dept1 = new Department("IT", "IT Department", null) { Id = Guid.NewGuid() };
        var dept2 = new Department("HR", "HR Department", null) { Id = Guid.NewGuid() };

        // Create employees - some hired this month, various tenure
        var employees = new List<Employee>
        {
            CreateEmployee("John", "Doe", new DateOnly(2020, 1, 15), dept1), // ~6 years, active
            CreateEmployee("Jane", "Smith", new DateOnly(2023, 5, 20), dept1), // ~2.5 years, active
            CreateEmployee("Bob", "Johnson", new DateOnly(2025, 12, 1), dept2), // New hire this month, active
            CreateEmployee("Alice", "Williams", new DateOnly(2021, 8, 10), dept2), // ~4 years, active
            CreateEmployeeWithTermination("Tom", "Brown", new DateOnly(2019, 3, 1), new DateOnly(2025, 11, 30), dept1), // Terminated
        };

        var employeeIds = employees.Select(e => e.Id).ToList();

        // Create attendance records for today
        var attendanceRecords = new List<AttendanceRecord>
        {
            CreateAttendanceRecord(employees[0].Id, todayDate, new TimeOnly(9, 0), false), // On time
            CreateAttendanceRecord(employees[1].Id, todayDate, new TimeOnly(9, 35), true),  // Late
            // employees[2] and [3] haven't checked in
        };

        // Create absence requests
        var absenceRequests = new List<AbsenceRequest>
        {
            CreateAbsenceRequest(employees[2].Id, todayDate, todayDate, RequestStatus.Approved, RequestType.Sick),
            CreateAbsenceRequest(employees[3].Id, todayDate, todayDate, RequestStatus.Approved, RequestType.Remote)
        };

        var employeesMock = employees.AsQueryable().BuildMockDbSet();
        var attendanceMock = attendanceRecords.AsQueryable().BuildMockDbSet();
        var absenceMock = absenceRequests.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Employees).Returns(employeesMock.Object);
        _contextMock.Setup(c => c.AttendanceRecords).Returns(attendanceMock.Object);
        _contextMock.Setup(c => c.AbsenceRequests).Returns(absenceMock.Object);

        var request = new GetEmployeeOverviewRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        // Statistics
        Assert.Equal(5, result.statistics.totalEmployees); // All 5 employees
        Assert.Equal(4, result.statistics.activeEmployees); // Tom is terminated, so only 4 active
        Assert.Equal(1, result.statistics.newHiresThisMonth); // Bob hired in December
        
        // Attendance
        Assert.Equal(5, result.attendance.totalEmployees);
        Assert.Equal(2, result.attendance.present); // John and Jane checked in
        Assert.Equal(1, result.attendance.late); // Jane was late
        Assert.Equal(1, result.attendance.onLeave); // Bob on sick leave
        Assert.Equal(1, result.attendance.remoteWorking); // Alice working remote
        Assert.Equal(1, result.attendance.absent); // Tom (terminated, not checked in, no leave)
    }

    [Fact]
    public async Task Handle_ShouldReturnUpcomingBirthdays_InNext30Days()
    {
        // Arrange
        var today = new DateTime(2025, 12, 15);
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(today);

        var dept = new Department("IT", "IT Department", null) { Id = Guid.NewGuid() };

        // Create employees with birthdays at different times
        var employee1 = CreateEmployeeWithBirthday("John", "Doe", new DateOnly(1990, 12, 20), dept); // In 5 days
        var employee2 = CreateEmployeeWithBirthday("Jane", "Smith", new DateOnly(1985, 1, 10), dept); // In 26 days (next year)
        var employee3 = CreateEmployeeWithBirthday("Bob", "Johnson", new DateOnly(1995, 3, 15), dept); // In ~90 days (should not appear)
        var employee4 = CreateEmployeeWithBirthday("Alice", "Williams", new DateOnly(1992, 12, 15), dept); // Today

        var employees = new List<Employee> { employee1, employee2, employee3, employee4 };

        var employeesMock = employees.AsQueryable().BuildMockDbSet();
        var attendanceMock = new List<AttendanceRecord>().AsQueryable().BuildMockDbSet();
        var absenceMock = new List<AbsenceRequest>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Employees).Returns(employeesMock.Object);
        _contextMock.Setup(c => c.AttendanceRecords).Returns(attendanceMock.Object);
        _contextMock.Setup(c => c.AbsenceRequests).Returns(absenceMock.Object);

        var request = new GetEmployeeOverviewRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.upcomingBirthdays);
        Assert.Equal(3, result.upcomingBirthdays.Count); // Should include birthdays within 30 days
        
        // Verify they're sorted by date
        Assert.True(result.upcomingBirthdays[0].date <= result.upcomingBirthdays[1].date);
        Assert.True(result.upcomingBirthdays[1].date <= result.upcomingBirthdays[2].date);
        
        // Verify ages are calculated correctly
        var aliceBirthday = result.upcomingBirthdays.FirstOrDefault(b => b.employeeName == "Alice Williams");
        Assert.NotNull(aliceBirthday);
        Assert.Equal(33, aliceBirthday.age); // 2025 - 1992
    }

    private Employee CreateEmployee(string firstName, string lastName, DateOnly hireDate, Department department)
    {
        var schedule = new Schedule(new ScheduleDto
        {
            Name = "Default",
            Description = "Standard work schedule",
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            DaysOfWeek = DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday
        });

        var employee = new Employee(new EmployeeDto
        {
            FirstName = firstName,
            LastName = lastName,
            MiddleName = "Test",
            Email = $"{firstName.ToLower()}.{lastName.ToLower()}@test.com",
            PassportNumber = "AA1234567",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            Nationality = "Test",
            Gender = Gender.Male,
            Pinfl = "12345678901234",
            PensionFundNumber = "1234567890",
            TaxIdentificationNumber = "1234567890",
            PhoneNumber = "1234567890",
            Address = new Address("Region", "Street", "House", "Apartment", "Full Address"),
            HireDate = hireDate,
            DepartmentId = department.Id,
            PositionId = Guid.NewGuid(),
            Salary = 1000,
            ScheduleId = schedule.Id
        });

        employee.GetType().GetProperty("Id")!.SetValue(employee, Guid.NewGuid());
        employee.GetType().GetProperty("WorkSchedule")!.SetValue(employee, schedule);
        employee.GetType().GetProperty("Department")!.SetValue(employee, department);
        employee.GetType().GetProperty("IsDeleted")!.SetValue(employee, false);
        employee.GetType().GetProperty("TerminationDate")!.SetValue(employee, null);
        
        return employee;
    }

    private Employee CreateEmployeeWithBirthday(string firstName, string lastName, DateOnly dateOfBirth, Department department)
    {
        var schedule = new Schedule(new ScheduleDto
        {
            Name = "Default",
            Description = "Standard work schedule",
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            DaysOfWeek = DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday
        });

        var employee = new Employee(new EmployeeDto
        {
            FirstName = firstName,
            LastName = lastName,
            MiddleName = "Test",
            Email = $"{firstName.ToLower()}.{lastName.ToLower()}@test.com",
            PassportNumber = "AA1234567",
            DateOfBirth = dateOfBirth,
            Nationality = "Test",
            Gender = Gender.Male,
            Pinfl = "12345678901234",
            PensionFundNumber = "1234567890",
            TaxIdentificationNumber = "1234567890",
            PhoneNumber = "1234567890",
            Address = new Address("Region", "Street", "House", "Apartment", "Full Address"),
            HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
            DepartmentId = department.Id,
            PositionId = Guid.NewGuid(),
            Salary = 1000,
            ScheduleId = schedule.Id
        });

        employee.GetType().GetProperty("Id")!.SetValue(employee, Guid.NewGuid());
        employee.GetType().GetProperty("WorkSchedule")!.SetValue(employee, schedule);
        employee.GetType().GetProperty("Department")!.SetValue(employee, department);
        employee.GetType().GetProperty("TerminationDate")!.SetValue(employee, null);
        
        return employee;
    }

    private AttendanceRecord CreateAttendanceRecord(Guid employeeId, DateOnly date, TimeOnly checkIn, bool isLate)
    {
        var dto = new AttendanceRecordDto
        {
            EmployeeId = employeeId,
            Date = date,
            CheckIn = checkIn,
            IsLate = isLate,
            TotalMinutes = 0,
            OvertimeMinutes = 0,
            IsEarlyDeparture = false
        };

        return new AttendanceRecord(dto);
    }

    private AbsenceRequest CreateAbsenceRequest(Guid employeeId, DateOnly startDate, DateOnly endDate, RequestStatus status, RequestType requestType)
    {
        var dto = new AbsenceRequestDto
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            RequestType = requestType,
            RequestStatus = status,
            Reason = "Test reason"
        };

        return new AbsenceRequest(dto);
    }

    private Employee CreateEmployeeWithTermination(string firstName, string lastName, DateOnly hireDate, DateOnly terminationDate, Department department)
    {
        var schedule = new Schedule(new ScheduleDto
        {
            Name = "Default",
            Description = "Standard work schedule",
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            DaysOfWeek = DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday
        });

        var employee = new Employee(new EmployeeDto
        {
            FirstName = firstName,
            LastName = lastName,
            MiddleName = "Test",
            Email = $"{firstName.ToLower()}.{lastName.ToLower()}@test.com",
            PassportNumber = "AA1234567",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            Nationality = "Test",
            Gender = Gender.Male,
            Pinfl = "12345678901234",
            PensionFundNumber = "1234567890",
            TaxIdentificationNumber = "1234567890",
            PhoneNumber = "1234567890",
            Address = new Address("Region", "Street", "House", "Apartment", "Full Address"),
            HireDate = hireDate,
            TerminationDate = terminationDate,
            DepartmentId = department.Id,
            PositionId = Guid.NewGuid(),
            Salary = 1000,
            ScheduleId = schedule.Id
        });

        employee.GetType().GetProperty("Id")!.SetValue(employee, Guid.NewGuid());
        employee.GetType().GetProperty("WorkSchedule")!.SetValue(employee, schedule);
        employee.GetType().GetProperty("Department")!.SetValue(employee, department);
        employee.GetType().GetProperty("IsDeleted")!.SetValue(employee, false);
        
        return employee;
    }
}
