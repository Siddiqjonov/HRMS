using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Departments.AssignDepartmentManager;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Departments;

public class AssignDepartmentManagerRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly AssignDepartmentManagerRequestHandler _handler;

    public AssignDepartmentManagerRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new AssignDepartmentManagerRequestHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAssignManager_WhenDepartmentHasNoManager()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();

        var department = new Department("IT", "Tech", null) { Id = departmentId };
        var employee = new Employee(new EmployeeDto()) { Id = employeeId };

        var departmentsList = new List<Department> { department }.AsQueryable().BuildMockDbSet();
        var employeesList = new List<Employee> { employee }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);
        _contextMock.Setup(c => c.Employees).Returns(employeesList.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new AssignDepartmentManagerRequest(departmentId, employeeId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(employeeId, department.ManagerId);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var employeesList = new List<Employee> { new Employee(new EmployeeDto()) { Id = Guid.NewGuid() } }
            .AsQueryable().BuildMockDbSet();
        var departmentsList = new List<Department>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);
        _contextMock.Setup(c => c.Employees).Returns(employeesList.Object);

        var request = new AssignDepartmentManagerRequest(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        var department = new Department("IT", "Tech", null)
        {
            Id = departmentId,
        };

        var departmentsList = new List<Department> { department }.AsQueryable().BuildMockDbSet();
        var employeesList = new List<Employee>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);
        _contextMock.Setup(c => c.Employees).Returns(employeesList.Object);

        var request = new AssignDepartmentManagerRequest(departmentId, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldOverwriteExistingManager()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var originalManagerId = Guid.NewGuid();
        var newEmployeeId = Guid.NewGuid();

        var department = new Department("IT", "Tech", originalManagerId) { Id = departmentId };
        var employee = new Employee(new EmployeeDto()) { Id = newEmployeeId };

        var departmentsList = new List<Department> { department }.AsQueryable().BuildMockDbSet();
        var employeesList = new List<Employee> { employee }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);
        _contextMock.Setup(c => c.Employees).Returns(employeesList.Object);

        var request = new AssignDepartmentManagerRequest(departmentId, newEmployeeId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(newEmployeeId, department.ManagerId);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}
