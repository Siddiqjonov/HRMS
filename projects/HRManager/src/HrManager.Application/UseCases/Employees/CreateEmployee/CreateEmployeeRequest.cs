using HrManager.Domain.Enums;
using HrManager.Domain.ValueObjects;

namespace HrManager.Application.UseCases.Employees.CreateEmployee;

public class CreateEmployeeRequest : IRequest<bool>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string MiddleName { get; set; }

    public string Email { get; set; }

    public string PassportNumber { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string Nationality { get; set; }

    public Gender Gender { get; set; }

    public string Pinfl { get; set; }

    public string PensionFundNumber { get; set; }

    public string TaxIdentificationNumber { get; set; }

    public string PhoneNumber { get; set; }

    public Address Address { get; set; }

    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public long Salary { get; set; }

    public Guid ScheduleId { get; set; }
}