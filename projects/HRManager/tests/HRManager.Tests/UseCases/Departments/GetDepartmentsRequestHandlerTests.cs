using AutoMapper;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Departments.GetDepartmentsWithPagination;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace HRManager.Tests.UseCases.Departments;

public class GetDepartmentsRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetDepartmentsWithPaginationRequestHandler _handler;

    public GetDepartmentsRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetDepartmentsWithPaginationRequestHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedDepartments_WhenDepartmentsExist()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department("Finance", "Finance Dept", null),
            new Department("HR", "HR Dept", null),
            new Department("IT", "IT Dept", null),
            new Department("Marketing", "Marketing Dept", null)
        };

        var mockQueryable = departments.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(mockQueryable.Object);

        var request = new GetDepartmentsWithPaginationRequest
        {
            pageNumber = 1, pageSize = 2
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(4, result.TotalCount);
        Assert.Contains(result.Items, d => d.name == "Finance");
        Assert.Contains(result.Items, d => d.name == "HR");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenPageOutOfRange()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department("Finance", "Finance Dept", null),
            new Department("HR", "HR Dept", null)
        };

        var mockQueryable = departments.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(mockQueryable.Object);

        var request = new GetDepartmentsWithPaginationRequest
        {
            pageNumber = 5, pageSize = 10
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(2, result.TotalCount);
    }
}
