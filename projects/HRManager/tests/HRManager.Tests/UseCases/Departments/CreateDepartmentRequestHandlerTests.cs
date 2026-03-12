using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Departments.CreateDepartment;
using HrManager.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Departments;

public class CreateDepartmentRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly IMapper _mapper;
    private readonly CreateDepartmentRequestHandler _handler;

    public CreateDepartmentRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateDepartmentProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new CreateDepartmentRequestHandler(_contextMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldCreateDepartment_WhenNameIsUniqueAndNoManager()
    {
        // Arrange
        var request = new CreateDepartmentRequest("HR", "Human Resources", null);

        var departmentsList = new List<Department>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);

        _contextMock.Setup(c => c.Departments.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.FromResult<EntityEntry<Department>>(null!));

        var employees = new List<Employee>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Employees).Returns(employees.Object);

        int? saveChangesResult = null;
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Callback<CancellationToken>(token => saveChangesResult = 1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        //Assert.Equal("HR", result.name);
        //Assert.Equal("Human Resources", result.description);
        //Assert.Equal(1, saveChangesResult);
        //Assert.Null(result.managerId);
        _contextMock.Verify(c => c.Departments.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenDepartmentNameAlreadyExists()
    {
        // Arrange
        var request = new CreateDepartmentRequest("HR", "Human Resources", null);

        var department = _mapper.Map<Department>(request);

        var existingDepartments = new List<Department> { department }
                                    .AsQueryable().BuildMockDbSet();

        var employees = new List<Employee>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Employees).Returns(employees.Object);

        _contextMock.Setup(c => c.Departments).Returns(existingDepartments.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenManagerAlreadyAssigned()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var request = new CreateDepartmentRequest("IT", "IT Department", managerId);

        var department = _mapper.Map<Department>(request);
        department.ManagerId = managerId;
        department.IsDeleted = false;

        var departmentsList = new List<Department> { department }
                        .AsQueryable().BuildMockDbSet();

        var employees = new List<Employee>
        {
            new Employee { Id = managerId } // make sure the manager exists
        }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Employees).Returns(employees.Object);

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateDepartment_WithManager()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var request = new CreateDepartmentRequest("IT", "IT Department", managerId);

        var departmentsList = new List<Department>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);

        _contextMock.Setup(c => c.Departments.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.FromResult<EntityEntry<Department>>(null!));

        var employees = new List<Employee>
        {
            new Employee { Id = managerId }
        }.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(c => c.Employees).Returns(employees.Object);


        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        //Assert.Equal("IT", result.name);
        //Assert.Equal(managerId, result.managerId);
    }

    [Fact]
    public async Task Handle_ShouldAllowCreation_WhenExistingDepartmentIsDeleted()
    {
        // Arrange
        var request = new CreateDepartmentRequest("HR", "Human Resources", null);

        var department = _mapper.Map<Department>(request);
        department.IsDeleted = true;

        var deletedDepartments = new List<Department>
        {
                department
        }
        .AsQueryable().Where(d => !d.IsDeleted).BuildMockDbSet();

        var employees = new List<Employee>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Employees).Returns(employees.Object);

        _contextMock.Setup(c => c.Departments).Returns(deletedDepartments.Object);

        _contextMock.Setup(c => c.Departments.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.FromResult<EntityEntry<Department>>(null!));

        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        //Assert.Equal("HR", result.name);
    }

}
