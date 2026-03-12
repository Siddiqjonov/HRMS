using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.AttendanceManagement.Correction;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.AttendanceManagement;

public class CorrectAttendanceRecordRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CorrectAttendanceRecordRequestHandler _handler;

    public CorrectAttendanceRecordRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new CorrectAttendanceRecordRequestHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAttendance_WhenValid()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var recordId = Guid.NewGuid();

        var employee = new Employee(new EmployeeDto()) { Id = empId, ScheduleId = scheduleId };
        var attendance = new AttendanceRecord(new AttendanceRecordDto
        {
            EmployeeId = empId,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckIn = new(9, 0),
            CheckOut = new(17, 0)
        })
        {
            Id = recordId,
            Employee = employee,
        };

        var schedule = new Schedule(new ScheduleDto { StartTime = new(9, 0), EndTime = new(18, 0) }) { Id = scheduleId };

        _contextMock.Setup(x => x.AttendanceRecords)
            .Returns(new List<AttendanceRecord> { attendance }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Schedules)
            .Returns(new List<Schedule> { schedule }.AsQueryable().BuildMockDbSet().Object);

        var request = new CorrectAttendanceRecordRequest(recordId, new(10, 0), new(19, 0));

        // Act
        await _handler.Handle(request, default);

        // Assert
        Assert.Equal(new TimeOnly(10, 0), attendance.CheckIn);
        Assert.Equal(new TimeOnly(19, 0), attendance.CheckOut);
        _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRecordNotFound()
    {
        // Arrange
        _contextMock.Setup(x => x.AttendanceRecords)
            .Returns(new List<AttendanceRecord>().AsQueryable().BuildMockDbSet().Object);

        var request = new CorrectAttendanceRecordRequest(Guid.NewGuid(), new(9, 0), new(18, 0));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, default));
    }

    [Fact]
    public async Task Handle_ShouldKeepExistingValues_WhenRequestHasNoNewTimes()
    {
        // Arrange
        var empId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var recordId = Guid.NewGuid();

        var employee = new Employee(new EmployeeDto()) { Id = empId, ScheduleId = scheduleId };
        var schedule = new Schedule(new ScheduleDto { StartTime = new(9, 0), EndTime = new(18, 0) }) { Id = scheduleId };
        var attendance = new AttendanceRecord(new AttendanceRecordDto
        {
            EmployeeId = empId,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckIn = new(9, 0),
            CheckOut = new(17, 0),
        })
        {
            Id = recordId,
            Employee = employee,
        };

        _contextMock.Setup(x => x.AttendanceRecords)
            .Returns(new List<AttendanceRecord> { attendance }.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Schedules)
            .Returns(new List<Schedule> { schedule }.AsQueryable().BuildMockDbSet().Object);

        var request = new CorrectAttendanceRecordRequest(recordId, null, null);

        // Act
        await _handler.Handle(request, default);

        // Assert (should not change values)
        Assert.Equal(new TimeOnly(9, 0), attendance.CheckIn);
        Assert.Equal(new TimeOnly(17, 0), attendance.CheckOut);
        _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
