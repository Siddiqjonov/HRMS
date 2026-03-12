using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;

namespace HrManager.Application.UseCases.Employees.UpdateEmployee;

public record UpdateEmployeeRequest(
    Guid Id,
    string FirstName,
    string LastName,
    string MiddleName,
    string Email,
    string PassportNumber,
    DateOnly DateOfBirth,
    string Nationality,
    Gender Gender,
    string Pinfl,
    string PensionFundNumber,
    string TaxIdentificationNumber,
    string PhoneNumber,
    Address Address,
    DateOnly HireDate,
    DateOnly? TerminationDate,
    Guid DepartmentId,
    Guid PositionId,
    long Salary,
    Guid ScheduleId
) : IRequest<bool>;
