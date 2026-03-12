using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.Employees.DeleteEmployee;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Employees.DeleteEmployeeTests;

public class DeleteEmployeeRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IDateTimeService> _mockDateTimeService;
    private readonly DeleteEmployeeRequestHandler _handler;

    public DeleteEmployeeRequestHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockDateTimeService = new Mock<IDateTimeService>();

        _mockCurrentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _mockDateTimeService.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        _handler = new DeleteEmployeeRequestHandler(
            _mockContext.Object,
            _mockCurrentUserService.Object,
            _mockDateTimeService.Object
            );
    }

    [Fact]
    public async Task Handle_Should_Delete_Employee_When_Found()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = new Employee(new EmployeeDto())
        {
            Id = employeeId
        };


        var employees = new[] { employee }.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.Employees).Returns(employees.Object);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(new DeleteEmployeeRequest(employeeId), CancellationToken.None);

        // Assert - Soft delete sets IsDeleted flag
        Assert.True(employee.IsDeleted);
        Assert.NotNull(employee.DeletedBy);
        Assert.NotEqual(DateTime.MinValue, employee.DeletedOnUtc);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Employee_Not_Found()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        var employees = new List<Employee>().AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Employees).Returns(employees.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteEmployeeRequest(employeeId), CancellationToken.None));
    }
}
