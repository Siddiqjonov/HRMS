using FluentValidation.TestHelper;
using HrManager.Application.UseCases.Employees.CreateEmployee;
using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;

namespace HRManager.Tests.UseCases.Employees.CreateEmployeeTests;

public class CreateEmployeeRequestValidatorTests
{
    private readonly CreateEmployeeRequestValidator _validator = new();

    private static CreateEmployeeRequest GetValidRequest()
    {
        return new CreateEmployeeRequest
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
            Address = new Address("Tashkent", "Street 1", "10A", "5", "Tashkent, Street 1, House 10A, Apt 5"),
            HireDate = DateOnly.FromDateTime(DateTime.Today),
            TerminationDate = null,
            DepartmentId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Salary = 1000,
            ScheduleId = Guid.NewGuid(),
        };
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var employee = GetValidRequest();
        employee.FirstName = string.Empty;
        var result = _validator.TestValidate(employee);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_DateOfBirth_Is_In_Future()
    {
        var employee = GetValidRequest();
        employee.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var result = _validator.TestValidate(employee);
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var emplopyee = GetValidRequest();
        var result = _validator.TestValidate(emplopyee);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
