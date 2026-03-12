using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Departments.GetDepartment;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Departments;

public class GetDepartmentByIdRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetDepartmentByIdRequestHandler _handler;

    public GetDepartmentByIdRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetDepartmentByIdRequestHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var request = new GetDepartmentByIdRequest(Guid.NewGuid());

        var departments = new List<Department>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departments.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnDepartmentDetails_WhenDepartmentExists()
    {
        // Arrange
        var depId = Guid.NewGuid();
        var department = new Department("IT", "Information Technology", null)
        {
            Id = depId
        };
        department.Employees.Add(new Employee(new EmployeeDto()));
        department.Employees.Add(new Employee(new EmployeeDto()));

        var departments = new List<Department> { department }
            .AsQueryable()
            .BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departments.Object);

        var request = new GetDepartmentByIdRequest(depId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("IT", result.name);
        Assert.Equal("Information Technology", result.description);
    }
}
