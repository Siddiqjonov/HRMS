using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Departments.RemoveDepartmentManager;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Departments;

public class RemoveDepartmentManagerRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly RemoveDepartmentManagerRequestHandler _handler;

    public RemoveDepartmentManagerRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new RemoveDepartmentManagerRequestHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var departmentsList = new List<Department>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);

        var request = new RemoveDepartmentManagerRequest(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldRemoveManager_WhenManagerExists()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var managerId = Guid.NewGuid();

        var department = new Department("IT", "Tech", managerId) { Id = departmentId };
        var departmentsList = new List<Department> { department }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new RemoveDepartmentManagerRequest(departmentId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Null(department.ManagerId);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldDoNothing_WhenNoManagerExists()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = new Department("IT", "Tech", null) { Id = departmentId };
        var departmentsList = new List<Department> { department }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new RemoveDepartmentManagerRequest(departmentId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Null(department.ManagerId);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
