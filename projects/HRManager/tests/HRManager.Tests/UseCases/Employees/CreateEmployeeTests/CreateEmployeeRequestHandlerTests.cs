    using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Employees.CreateEmployee;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using HrManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Employees.CreateEmployeeTests;

public class CreateEmployeeRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly IMapper _mapper;
    private readonly CreateEmployeeRequestHandler _handler;

    public CreateEmployeeRequestHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EmployeeProfile>();
        });
        _mapper = config.CreateMapper();

        _mockContext = new Mock<IApplicationDbContext>();
        _handler = new CreateEmployeeRequestHandler(_mockContext.Object, _mapper);
    }

    [Fact]
    public async Task Handle_Should_Create_Employee_And_Return_Response()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var positionId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();

        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PassportNumber = "UNIQUE123",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            Nationality = "Uzbek",
            Gender = HrManager.Domain.Enums.Gender.Male,
            Pinfl = "12345678901234",
            HireDate = DateOnly.FromDateTime(DateTime.Today),
            DepartmentId = departmentId,
            PositionId = positionId,
            Salary = 1200,
            ScheduleId = scheduleId,
            Address = new Address("Tashkent", "Street 1", "10A", "5", "Tashkent, Street 1, House 10A, Apt 5")
        };

        var employeeList = new List<Employee>().AsQueryable().BuildMockDbSet();
        var departmentList = new List<Department>
        {
            new Department("IT", "Tech Department", null) { Id = departmentId }
        }.AsQueryable().BuildMockDbSet();
        var positionList = new List<Position>
        {
            new Position("Developer", departmentId, 1000, 5000) { Id = positionId }
        }.AsQueryable().BuildMockDbSet();
        var scheduleList = new List<Schedule>
        {
            new Schedule(new ScheduleDto { StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) }) { Id = scheduleId }
        }.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.Employees).Returns(employeeList.Object);
        _mockContext.Setup(c => c.Departments).Returns(departmentList.Object);
        _mockContext.Setup(c => c.Positions).Returns(positionList.Object);
        _mockContext.Setup(c => c.Schedules).Returns(scheduleList.Object);

        _mockContext.Setup(c => c.Employees.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.FromResult<EntityEntry<Employee>>(null!));

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Employees.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowConflictException_When_PassportNumberExists()
    {
        // Arrange
        var existingEmployee = new Employee(new EmployeeDto
        {

            PassportNumber = "EXIST123",
            FirstName = "Existing",
            LastName = "Employee",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            Gender = HrManager.Domain.Enums.Gender.Male,
            Nationality = "Uzbek",
            Pinfl = "12345678901234",
            DepartmentId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Salary = 1000,
            ScheduleId = Guid.NewGuid(),
            Address = new Address(
         "Tashkent", "Street 1", "10A", "5", "Tashkent, Street 1, House 10A, Apt 5")
        })
        {
            Id = Guid.NewGuid()
        };

        var employeeList = new List<Employee> { existingEmployee }.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Employees).Returns(employeeList.Object);

        var request = new CreateEmployeeRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PassportNumber = "EXIST123",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            Nationality = "Uzbek",
            Gender = HrManager.Domain.Enums.Gender.Female,
            Pinfl = "98765432101234",
            HireDate = DateOnly.FromDateTime(DateTime.Today),
            DepartmentId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Salary = 1500,
            ScheduleId = Guid.NewGuid(),
            Address = new Address("Tashkent", "Street 2", "20B", "3", "Tashkent, Street 2, House 20B, Apt 3")
        };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, CancellationToken.None));

        _mockContext.Verify(c => c.Employees.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

