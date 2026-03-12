using AutoMapper;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Employees.UpdateEmployee;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Employees.UpdateEmployeeTests;

public class UpdateEmployeeRequestHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly IMapper _mapper;
    private readonly UpdateEmployeeRequestHandler _handler;

    public UpdateEmployeeRequestHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdateEmployeeProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new UpdateEmployeeRequestHandler(_mockContext.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEmployee_WhenEmployeeExists()
    {
        // Arrange
        var deptId = Guid.NewGuid();
        var posId = Guid.NewGuid();
        var schedId = Guid.NewGuid();

        var employee = new Employee(new EmployeeDto
        {
            FirstName = "Old",
            LastName = "Name",
            MiddleName = "Middle",
            PassportNumber = "AB1234567",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            Nationality = "Uzbek",
            Gender = Gender.Male,
            Pinfl = "12345678901234",
            PensionFundNumber = "PF123",
            TaxIdentificationNumber = "TIN123",
            PhoneNumber = "+998901234567",
            Address = new Address("Tashkent", "Street", "1", "1", "Full"),
            HireDate = DateOnly.FromDateTime(DateTime.Today),
            DepartmentId = deptId,
            PositionId = posId,
            Salary = 1000,
            ScheduleId = schedId,
        })
        {
            Id = Guid.NewGuid()
        };

        var employees = new List<Employee> { employee }.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Employees).Returns(employees.Object);

        var request = new UpdateEmployeeRequest(
            Id: employee.Id,
            FirstName: "New",
            LastName: "Surname",
            MiddleName: "M",
            Email: "admin@gmail.com",
            PassportNumber: "AB1234567",
            DateOfBirth: default,
            Nationality: "Uzbek",
            Gender: 0,
            Pinfl: "asdbasuydviasd",
            PensionFundNumber: "PF999",
            TaxIdentificationNumber: "TIN999",
            PhoneNumber: "+998909999999",
            Address: new Address("Samarkand", "Street 2", "10A", "5", "Full"),
            HireDate: DateOnly.FromDateTime(DateTime.Today),
            TerminationDate: null,
            DepartmentId: deptId,
            PositionId: posId,
            Salary: 2000,
            ScheduleId: schedId
        );

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        //Assert.Equal("New", result.FirstName);
        //Assert.Equal("Surname", result.LastName);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var employees = new List<Employee>().AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Employees).Returns(employees.Object);

        var request = new UpdateEmployeeRequest(
            Id: Guid.NewGuid(),
            FirstName: "New",
            LastName: "Surname",
            MiddleName: "M",
            Email: "employee@gamil.com",
            PassportNumber: "AB1234567",
            DateOfBirth: default,
            Nationality: "Uzbek",
            Gender: 0,
            Pinfl: "asdbasuydviasd",
            PensionFundNumber: "PF999",
            TaxIdentificationNumber: "TIN999",
            PhoneNumber: "+998909999999",
            Address: new Address("Samarkand", "Street 2", "10A", "5", "Full"),
            HireDate: DateOnly.FromDateTime(DateTime.Today),
            TerminationDate: null,
            DepartmentId: Guid.NewGuid(),
            PositionId: Guid.NewGuid(),
            Salary: 2000,
            ScheduleId: Guid.NewGuid()
        );

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}
