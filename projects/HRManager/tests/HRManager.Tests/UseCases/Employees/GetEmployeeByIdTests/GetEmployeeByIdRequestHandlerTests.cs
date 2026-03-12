using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Employees.GetEmployee;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Employees.GetEmployeeByIdTests;

public class GetEmployeeByIdRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly IMapper _mapper;
    private readonly GetEmployeeByIdRequestHandler _handler;
    public GetEmployeeByIdRequestHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetEmployeeByIdProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetEmployeeByIdRequestHandler(_mockContext.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmployee_WhenFound()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var dto = new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "Middle",
            PassportNumber = "AB1234567",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            Nationality = "Uzbek",
            Gender = HrManager.Domain.Enums.Gender.Male,
            Pinfl = "12345678901234",
            PensionFundNumber = "PF12345",
            TaxIdentificationNumber = "TIN12345",
            PhoneNumber = "+998901234567",
            Address = new HrManager.Domain.ValueObjects.Address(
            "Tashkent", "Street 1", "10A", "5",
            "Tashkent, Street 1, House 10A, Apt 5"),
            HireDate = DateOnly.FromDateTime(DateTime.Today),
            TerminationDate = default,
            DepartmentId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Salary = 1000,
            ScheduleId = Guid.NewGuid(),
        };

        var employee = new Employee(dto)
        {
            Id = employeeId
        };

        var department = new Department("IT", "Tech Department", Guid.NewGuid());
        var position = new Position("Developer", Guid.NewGuid(), 5000, 10000);

        typeof(Employee).GetProperty(nameof(Employee.Department))!
            .SetValue(employee, department);

        typeof(Employee).GetProperty(nameof(Employee.Position))!
            .SetValue(employee, position);

        var employees = new[] { employee }.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.Employees).Returns(employees.Object);

        var request = new GetEmployeeByIdRequest(employeeId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }


    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenEmployeeNotFound()
    {
        // Arrange
        var employees = Array.Empty<Employee>().AsQueryable();
        var dbSetMock = employees.BuildMockDbSet();

        _mockContext.Setup(c => c.Employees).Returns(dbSetMock.Object);

        var request = new GetEmployeeByIdRequest(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}
