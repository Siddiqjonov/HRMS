using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.Departments.GetDepartmentOverview;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Departments;

public class GetDepartmentOverviewRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IDateTimeService> _dateTimeServiceMock;
    private readonly GetDepartmentOverviewRequestHandler _handler;

    public GetDepartmentOverviewRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _dateTimeServiceMock = new Mock<IDateTimeService>();
        
        _handler = new GetDepartmentOverviewRequestHandler(
            _contextMock.Object, 
            _dateTimeServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var request = new GetDepartmentOverviewRequest(Guid.NewGuid());
        var departments = new List<Department>().AsQueryable().BuildMockDbSet();
        
        _contextMock.Setup(c => c.Departments).Returns(departments.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnOverview_WithCorrectAttendanceStatus()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var today = new DateTime(2025, 12, 14);
        var todayDate = DateOnly.FromDateTime(today);
        
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(today);

        // Create employees with explicit IDs
        var employee1Id = Guid.NewGuid();
        var employee2Id = Guid.NewGuid();
        var employee3Id = Guid.NewGuid();
        var employee4Id = Guid.NewGuid();
        
        var employee1 = CreateEmployee("John", "Doe", employee1Id); // Present
        var employee2 = CreateEmployee("Jane", "Smith", employee2Id); // Late
        var employee3 = CreateEmployee("Bob", "Johnson", employee3Id); // Absent
        var employee4 = CreateEmployee("Alice", "Williams", employee4Id); // On Leave

        var department = new Department("IT", "IT Department", null)
        {
            Id = departmentId
        };
        department.Employees.Add(employee1);
        department.Employees.Add(employee2);
        department.Employees.Add(employee3);
        department.Employees.Add(employee4);

        // Create attendance records using the same employee IDs
        var attendanceRecords = new List<AttendanceRecord>
        {
            CreateAttendanceRecord(employee1Id, todayDate, new TimeOnly(9, 0), false), // On time
            CreateAttendanceRecord(employee2Id, todayDate, new TimeOnly(9, 30), true)  // Late
            // employee3 has no attendance record (absent)
            // employee4 is on leave
        };

        // Create absence request for employee4
        var absenceRequest = CreateAbsenceRequest(employee4Id, todayDate, todayDate, RequestStatus.Approved);
        absenceRequest.GetType().GetProperty("Employee")!.SetValue(absenceRequest, employee4);
        
        var absenceRequests = new List<AbsenceRequest>
        {
            absenceRequest
        };

        var departments = new List<Department> { department }
            .AsQueryable()
            .BuildMockDbSet();

        var attendanceMock = attendanceRecords.AsQueryable().BuildMockDbSet();
        var absenceMock = absenceRequests.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departments.Object);
        _contextMock.Setup(c => c.AttendanceRecords).Returns(attendanceMock.Object);
        _contextMock.Setup(c => c.AbsenceRequests).Returns(absenceMock.Object);

        var request = new GetDepartmentOverviewRequest(departmentId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("IT", result.name);
        Assert.Equal("IT Department", result.description);
        Assert.Equal(4, result.totalEmployees);
        Assert.Equal(4, result.teamMembers.Count);
        
        // Count statuses from the result
        var actualPresentCount = result.teamMembers.Count(m => m.status == "present");
        var actualLateCount = result.teamMembers.Count(m => m.status == "late");
        var actualAbsentCount = result.teamMembers.Count(m => m.status == "absent");
        var actualOnLeaveCount = result.teamMembers.Count(m => m.status == "on-leave");
        
        Assert.Equal(1, actualPresentCount);
        Assert.Equal(1, actualLateCount);
        Assert.Equal(1, actualAbsentCount);
        Assert.Equal(1, actualOnLeaveCount);
        
        // presentToday should include both on-time and late employees (2 total)
        Assert.Equal(2, result.presentToday);
        Assert.Equal(1, result.absentToday);
        Assert.Equal(1, result.lateToday);
        Assert.Equal(1, result.onLeaveToday);

        // Verify team members
        var presentMember = result.teamMembers.First(m => m.id == employee1Id);
        Assert.Equal("present", presentMember.status);
        Assert.Equal("09:00", presentMember.checkInTime);

        var lateMember = result.teamMembers.First(m => m.id == employee2Id);
        Assert.Equal("late", lateMember.status);
        Assert.Equal("09:30", lateMember.checkInTime);

        var absentMember = result.teamMembers.First(m => m.id == employee3Id);
        Assert.Equal("absent", absentMember.status);
        Assert.Null(absentMember.checkInTime);

        var onLeaveMember = result.teamMembers.First(m => m.id == employee4Id);
        Assert.Equal("on-leave", onLeaveMember.status);

        // Verify upcoming leaves
        Assert.NotNull(result.upcomingLeaves);
        Assert.Single(result.upcomingLeaves);
        var leave = result.upcomingLeaves.First();
        Assert.Equal("Alice Williams", leave.employeeName);
        Assert.Equal("Vacation", leave.leaveType);
        Assert.Equal(todayDate, leave.startDate);
        Assert.Equal(todayDate, leave.endDate);
        Assert.Equal("Approved", leave.status);
    }

    [Fact]
    public async Task Handle_ShouldIncludeUpcomingLeaves_WithinNext30Days()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var today = new DateTime(2025, 12, 14);
        var todayDate = DateOnly.FromDateTime(today);
        
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(today);

        var employee1Id = Guid.NewGuid();
        var employee2Id = Guid.NewGuid();
        var employee3Id = Guid.NewGuid();
        
        var employee1 = CreateEmployee("John", "Doe", employee1Id);
        var employee2 = CreateEmployee("Jane", "Smith", employee2Id);
        var employee3 = CreateEmployee("Bob", "Johnson", employee3Id);

        var department = new Department("IT", "IT Department", null)
        {
            Id = departmentId
        };
        department.Employees.Add(employee1);
        department.Employees.Add(employee2);
        department.Employees.Add(employee3);

        // Create various absence requests
        var currentLeave = CreateAbsenceRequest(employee1Id, todayDate, todayDate.AddDays(2), RequestStatus.Approved, RequestType.Vacation);
        currentLeave.GetType().GetProperty("Employee")!.SetValue(currentLeave, employee1);
        
        var futureLeave = CreateAbsenceRequest(employee2Id, todayDate.AddDays(10), todayDate.AddDays(15), RequestStatus.Approved, RequestType.Sick);
        futureLeave.GetType().GetProperty("Employee")!.SetValue(futureLeave, employee2);
        
        var farFutureLeave = CreateAbsenceRequest(employee3Id, todayDate.AddDays(40), todayDate.AddDays(45), RequestStatus.Approved, RequestType.Remote);
        farFutureLeave.GetType().GetProperty("Employee")!.SetValue(farFutureLeave, employee3);
        
        var pendingLeave = CreateAbsenceRequest(employee1Id, todayDate.AddDays(5), todayDate.AddDays(7), RequestStatus.Pending, RequestType.Unpaid);
        pendingLeave.GetType().GetProperty("Employee")!.SetValue(pendingLeave, employee1);

        var absenceRequests = new List<AbsenceRequest>
        {
            currentLeave,
            futureLeave,
            farFutureLeave, // Should be excluded (beyond 30 days)
            pendingLeave    // Should be excluded (not approved)
        };

        var departments = new List<Department> { department }
            .AsQueryable()
            .BuildMockDbSet();

        var attendanceMock = new List<AttendanceRecord>().AsQueryable().BuildMockDbSet();
        var absenceMock = absenceRequests.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departments.Object);
        _contextMock.Setup(c => c.AttendanceRecords).Returns(attendanceMock.Object);
        _contextMock.Setup(c => c.AbsenceRequests).Returns(absenceMock.Object);

        var request = new GetDepartmentOverviewRequest(departmentId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.upcomingLeaves);
        
        // Should only include approved leaves within 30 days
        Assert.Equal(2, result.upcomingLeaves.Count);
        
        var leave1 = result.upcomingLeaves.First(l => l.employeeName == "John Doe");
        Assert.Equal("Vacation", leave1.leaveType);
        Assert.Equal(todayDate, leave1.startDate);
        Assert.Equal(todayDate.AddDays(2), leave1.endDate);
        
        var leave2 = result.upcomingLeaves.First(l => l.employeeName == "Jane Smith");
        Assert.Equal("Sick", leave2.leaveType);
        Assert.Equal(todayDate.AddDays(10), leave2.startDate);
        Assert.Equal(todayDate.AddDays(15), leave2.endDate);
        
        // Verify leaves are ordered by start date
        Assert.True(result.upcomingLeaves[0].startDate <= result.upcomingLeaves[1].startDate);
    }

    [Fact]
    public async Task Handle_ShouldReturnOverview_ForEmptyDepartment()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var today = new DateTime(2025, 12, 14);
        
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(today);

        var department = new Department("Marketing", "Marketing Department", null)
        {
            Id = departmentId
        };

        var departments = new List<Department> { department }
            .AsQueryable()
            .BuildMockDbSet();

        var attendanceMock = new List<AttendanceRecord>().AsQueryable().BuildMockDbSet();
        var absenceMock = new List<AbsenceRequest>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departments.Object);
        _contextMock.Setup(c => c.AttendanceRecords).Returns(attendanceMock.Object);
        _contextMock.Setup(c => c.AbsenceRequests).Returns(absenceMock.Object);

        var request = new GetDepartmentOverviewRequest(departmentId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.totalEmployees);
        Assert.Equal(0, result.presentToday);
        Assert.Equal(0, result.absentToday);
        Assert.Equal(0, result.lateToday);
        Assert.Equal(0, result.onLeaveToday);
        Assert.Empty(result.teamMembers);
        Assert.Empty(result.upcomingLeaves);
    }

    private Employee CreateEmployee(string firstName, string lastName, Guid? employeeId = null)
    {
        var id = employeeId ?? Guid.NewGuid();
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
            HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
            DepartmentId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Salary = 1000,
            ScheduleId = schedule.Id
        });

        // Set the ID using reflection
        employee.GetType().GetProperty("Id")!.SetValue(employee, id);
        employee.GetType().GetProperty("WorkSchedule")!.SetValue(employee, schedule);
        
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

    private AbsenceRequest CreateAbsenceRequest(Guid employeeId, DateOnly startDate, DateOnly endDate, RequestStatus status, RequestType? requestType = null)
    {
        var dto = new AbsenceRequestDto
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            RequestType = requestType ?? RequestType.Vacation,
            RequestStatus = status,
            Reason = "Test reason"
        };

        return new AbsenceRequest(dto);
    }
}

