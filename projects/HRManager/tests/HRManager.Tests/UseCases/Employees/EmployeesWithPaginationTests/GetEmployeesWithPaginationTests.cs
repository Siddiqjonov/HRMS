using AutoMapper;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.Employees.DeleteEmployee;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Dtos;
using HrManager.Domain.Entities;
using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;
using MockQueryable.Moq;
using Moq;

namespace HRManager.Tests.UseCases.Employees.EmployeesWithPaginationTests;

public class GetEmployeesWithPaginationTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly IMapper _mapper;
    private readonly GetEmployeesWithPaginationRequestHandler _handler;

    public GetEmployeesWithPaginationTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EmployeeProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetEmployeesWithPaginationRequestHandler(_mockContext.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedEmployees_WhenEmployeesExist()
    {
        // Arrange
        var dept = new Department("IT", "Tech Department", Guid.NewGuid());
        var pos = new Position("Software Engineer", Guid.NewGuid(), 5000, 10000);

        var dto1 = new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "Middle",
            Email = "john.doe@example.com",
            PassportNumber = "AB1234567",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            Nationality = "Uzbek",
            Gender = Gender.Male,
            Pinfl = "12345678901234",
            PensionFundNumber = "PF12345",
            TaxIdentificationNumber = "TIN12345",
            PhoneNumber = "+998901234567",
            Address = new Address("Tashkent", "Street 1", "10A", "5", "Full"),
            HireDate = DateOnly.FromDateTime(DateTime.Today),
            DepartmentId = dept.Id,
            PositionId = pos.Id,
            Salary = 1000,
            ScheduleId = Guid.NewGuid(),
        };

        var employee1 = new Employee(dto1) { Id = Guid.NewGuid() };

        typeof(Employee).GetProperty(nameof(Employee.Department))!
       .SetValue(employee1, dept);
        typeof(Employee).GetProperty(nameof(Employee.Position))!
            .SetValue(employee1, pos);

        var dto2 = new EmployeeDto
        {
            FirstName = "Alice",
            LastName = "Smith",
            MiddleName = dto1.MiddleName,
            Email = "alice.smith@example.com",
            PassportNumber = "XY9876543",
            DateOfBirth = dto1.DateOfBirth,
            Nationality = dto1.Nationality,
            Gender = dto1.Gender,
            Pinfl = "98765432109876",
            PensionFundNumber = dto1.PensionFundNumber,
            TaxIdentificationNumber = dto1.TaxIdentificationNumber,
            PhoneNumber = dto1.PhoneNumber,
            Address = dto1.Address,
            HireDate = dto1.HireDate,
            TerminationDate = dto1.TerminationDate,
            DepartmentId = dto1.DepartmentId,
            PositionId = dto1.PositionId,
            Salary = dto1.Salary,
            ScheduleId = dto1.ScheduleId,
        };

        var employee2 = new Employee(dto2) { Id = Guid.NewGuid() };
        typeof(Employee).GetProperty(nameof(Employee.Department))!
            .SetValue(employee2, dept);
        typeof(Employee).GetProperty(nameof(Employee.Position))!
            .SetValue(employee2, pos);

        var employees = new[] { employee1, employee2 }.AsQueryable().BuildMockDbSet();
        var departments = new[] { dept }.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.Employees).Returns(employees.Object);
        _mockContext.Setup(c => c.Departments).Returns(departments.Object);

        var request = new GetEmployeesWithPaginationRequest(pageNumber: 1, pageSize: 10);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items, e => e.FullName.Contains("John"));
        Assert.Contains(result.Items, e => e.DepartmentName == "IT");
        Assert.Contains(result.Items, e => e.PositionName == "Software Engineer");
    }
}
