using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Departments.CreateDepartment;
using HrManager.Application.UseCases.Departments.UpdateDepartment;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Departments;

public class UpdateDepartmentRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly IMapper _mapper;
    private readonly UpdateDepartmentRequestHandler _handler;

    public UpdateDepartmentRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdateDepartmentProfile>();
            cfg.AddProfile<CreateDepartmentProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new UpdateDepartmentRequestHandler(_contextMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDepartment_WhenValidRequest()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        var department = new Department("Old Name", "Old Description", null)
        {
            Id = departmentId
        };

        var realDepartmentsList = new List<Department> { department };
        var departmentsDbSet = realDepartmentsList.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsDbSet.Object);

        var request = new UpdateDepartmentRequest(departmentId, "New Name", "New Description", null);

        int? effectedEntity = null;
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Callback<CancellationToken>(t => effectedEntity = 1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        //Assert.Equal("New Name", result.name);
        //Assert.Equal("New Description", result.description);
        Assert.Equal(1, effectedEntity);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenNameAlreadyExists()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var existingDepartment = new Department("Existing Name", "Existing Description", null);

        var realDepartmentsList = new List<Department> { existingDepartment };
        var departmentsDbSet = realDepartmentsList.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsDbSet.Object);

        var request = new UpdateDepartmentRequest(departmentId, "Existing Name", "Desc", null);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenManagerAlreadyAssigned()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var managerId = Guid.NewGuid();

        var existingDepartment = new Department("Finance", "Finance Dept", managerId);

        var realDepartmentsList = new List<Department> { existingDepartment };
        var departmentsDbSet = realDepartmentsList.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsDbSet.Object);

        var request = new UpdateDepartmentRequest(departmentId, "HR", "Desc", managerId);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var realDepartmentsList = new List<Department>();
        var departmentsDbSet = realDepartmentsList.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsDbSet.Object);

        var request = new UpdateDepartmentRequest(Guid.NewGuid(), "HR", "Desc", null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
