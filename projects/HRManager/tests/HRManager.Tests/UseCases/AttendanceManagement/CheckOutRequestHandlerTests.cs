using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.AttendanceManagement.CheckOut;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.AttendanceManagement;

public class CheckOutRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CheckOutRequestHandler _handler;

    public CheckOutRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new CheckOutRequestHandler(_contextMock.Object, new DateTimeService());
    }

    [Fact]
    public async Task Handle_ShouldUpdateAttendance_WhenValid()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var employee = new Employee(new EmployeeDto()) { Id = empId, ScheduleId = scheduleId };
        var schedule = new Schedule(new ScheduleDto { StartTime = new(9, 0), EndTime = new(18, 0) }) { Id = scheduleId };

        var record = new AttendanceRecord(new AttendanceRecordDto
        {
            Id = Guid.NewGuid(),
            EmployeeId = empId,
            Date = today,
            CheckIn = new(9, 0)
        });

        _contextMock.Setup(x => x.Employees)
            .Returns(new List<Employee> { employee }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Schedules)
            .Returns(new List<Schedule> { schedule }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.AttendanceRecords)
            .Returns(new List<AttendanceRecord> { record }.AsQueryable().BuildMockDbSet().Object);

        // Act
        await _handler.Handle(new CheckOutRequest(empId), default);

        // Assert
        Assert.NotEqual(default, record.CheckOut);
        _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenEmployeeNotFound()
    {
        // Arrange
        _contextMock.Setup(x => x.Employees)
            .Returns(new List<Employee>().AsQueryable().BuildMockDbSet().Object);

        var request = new CheckOutRequest(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenScheduleNotFound()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var employee = new Employee(new EmployeeDto()) { Id = empId };

        _contextMock.Setup(x => x.Employees)
            .Returns(new List<Employee> { employee }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Schedules)
            .Returns(new List<Schedule>().AsQueryable().BuildMockDbSet().Object);

        var request = new CheckOutRequest(empId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenNoCheckInRecord()
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

        var request = new CheckOutRequest(empId);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenAlreadyCheckedOut()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var employee = new Employee(new EmployeeDto()) { Id = empId, ScheduleId = scheduleId };
        var schedule = new Schedule(new ScheduleDto { StartTime = new(9, 0), EndTime = new(18, 0) }) { Id = scheduleId };

        var record = new AttendanceRecord(new AttendanceRecordDto
        {
            Id = Guid.NewGuid(),
            EmployeeId = empId,
            Date = today,
            CheckIn = new(9, 0),
            CheckOut = new(17, 0)
        });

        _contextMock.Setup(x => x.Employees)
            .Returns(new List<Employee> { employee }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Schedules)
            .Returns(new List<Schedule> { schedule }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.AttendanceRecords)
            .Returns(new List<AttendanceRecord> { record }.AsQueryable().BuildMockDbSet().Object);

        var request = new CheckOutRequest(empId);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, default));
    }
}
