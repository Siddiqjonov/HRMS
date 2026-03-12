using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Departments.CreateDepartment;
using HrManager.Application.UseCases.Departments.DeleteDepartment;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Departments;

public class DeleteDepartmentRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly IMapper _mapper;
    private readonly DeleteDepartmentRequestHandler _handler;

    public DeleteDepartmentRequestHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateDepartmentProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new DeleteDepartmentRequestHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteDepartment_WhenNoEmployees()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var createRequest = new CreateDepartmentRequest("HR", "Human resources", null);
        var departemntToAdd = _mapper.Map<Department>(createRequest);
        departemntToAdd.Id = departmentId;
        var realDepartmentsList = new List<Department>
        {
            departemntToAdd,
        };

        var departmentsList = realDepartmentsList.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);

        var employeesList = new List<Employee>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Employees).Returns(employeesList.Object);

        _contextMock.Setup(c => c.Departments.Remove(It.IsAny<Department>()))
            .Callback<Department>(d => realDepartmentsList.Remove(d));

        int? effectedEntity = null;
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1)
            .Callback<CancellationToken>(token => effectedEntity = 1);

        var request = new DeleteDepartmentRequest(departmentId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _contextMock.Verify(c => c.Departments.Remove(It.IsAny<Department>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(1, effectedEntity);
        Assert.Equal(0, departmentsList.Object.Count());
        Assert.Empty(realDepartmentsList);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var departmentsList = new List<Department>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);

        var request = new DeleteDepartmentRequest(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenDepartmentHasEmployees()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        var createReques = new CreateDepartmentRequest("IT", "Information Technolagy", null);
        var department = _mapper.Map<Department>(createReques);
        department.Id = departmentId;
        
        department.Employees.Add(new Employee(new EmployeeDto()));

        var departmentsList = new List<Department>
        {
            department,
        }.AsQueryable().Where(d => !d.IsDeleted).BuildMockDbSet();
        _contextMock.Setup(c => c.Departments).Returns(departmentsList.Object);

        var employeesList = new List<Employee>() { new Employee(new EmployeeDto()) { DepartmentId = departmentId } }
                    .AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Employees).Returns(employeesList.Object);

        var request = new DeleteDepartmentRequest(departmentId);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
