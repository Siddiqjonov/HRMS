using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.AttendanceManagement.CheckIn;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HrManager.Tests.UseCases.AttendanceManagement;

public class CheckInRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CheckInRequestHandler _handler;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IDateTimeService> _dateTimeMock;

    public CheckInRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _emailServiceMock = new Mock<IEmailService>();
        _dateTimeMock = new Mock<IDateTimeService>();
        _handler = new CheckInRequestHandler(_contextMock.Object, _emailServiceMock.Object, _dateTimeMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddAttendance_WhenValid()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();

        var employee = new Employee(new EmployeeDto()) { Id = empId, ScheduleId = scheduleId };
        var schedule = new Schedule(new ScheduleDto { StartTime = new(9, 0), EndTime = new(18, 0) }) { Id = scheduleId };

        _contextMock.Setup(x => x.Employees)
            .Returns(new List<Employee> { employee }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Schedules)
            .Returns(new List<Schedule> { schedule }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.AttendanceRecords)
            .Returns(new List<AttendanceRecord>().AsQueryable().BuildMockDbSet().Object);

        _dateTimeMock.Setup(x => x.UtcNow).Returns(new DateTime(2025, 09, 30, 9, 30, 0));

        // Act
        await _handler.Handle(new CheckInRequest(empId), default);

        // Assert
        _contextMock.Verify(x => x.AttendanceRecords.AddAsync(It.IsAny<AttendanceRecord>(), default), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenAlreadyCheckedIn()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var now = new DateTime(2025, 9, 30, 9, 0, 0);
        var today = DateOnly.FromDateTime(now);

        _contextMock.Setup(x => x.AttendanceRecords).Returns(
            new List<AttendanceRecord>
            {
            new(new AttendanceRecordDto { EmployeeId = empId, Date = today, CheckIn = new(9, 0) })
            }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Employees).Returns(new List<Employee> { new(new EmployeeDto()) { Id = empId } }
                .AsQueryable().BuildMockDbSet().Object);
        _contextMock.Setup(x => x.Schedules)
                .Returns(new List<Schedule> { new(new ScheduleDto { StartTime = new(9, 0), EndTime = new(18, 0) }) }
                .AsQueryable().BuildMockDbSet().Object);

        _dateTimeMock.Setup(x => x.UtcNow).Returns(now);

        var request = new CheckInRequest(empId);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenEmployeeNotFound()
    {
        // Arrange
        var employees = new List<Employee>().AsQueryable().BuildMockDbSet().Object;
        var attendanceRecords = new List<AttendanceRecord>().AsQueryable().BuildMockDbSet().Object;

        _contextMock.Setup(x => x.Employees).Returns(employees);
        _contextMock.Setup(x => x.AttendanceRecords).Returns(attendanceRecords);

        var request = new CheckInRequest(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, default));
    }

    [Fact]
    public async Task Handle_ShouldAllowCheckIn_EvenIfEarly()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var startTime = new TimeOnly(12, 0, 0);  // Schedule starts at noon
        var earlyCheckInTime = new DateTime(2025, 9, 30, 8, 0, 0);  // But checking in at 8 AM

        var employee = new Employee(new EmployeeDto()) { Id = empId, ScheduleId = scheduleId };
        var schedule = new Schedule(new ScheduleDto { StartTime = startTime, EndTime = new(18, 0) }) { Id = scheduleId };

        _contextMock.Setup(x => x.Employees)
            .Returns(new List<Employee> { employee }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Schedules)
            .Returns(new List<Schedule> { schedule }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.AttendanceRecords)
            .Returns(new List<AttendanceRecord>().AsQueryable().BuildMockDbSet().Object);

        _dateTimeMock.Setup(x => x.UtcNow).Returns(earlyCheckInTime);

        var request = new CheckInRequest(empId);

        // Act
        await _handler.Handle(request, default);

        // Assert - Should allow early check-in
        _contextMock.Verify(x => x.AttendanceRecords.AddAsync(It.IsAny<AttendanceRecord>(), default), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
