using HrManager.Domain.Dtos;

namespace HrManager.Domain.Entities;

public class Employee : SoftDeletableAuditableEntity
{
    public Employee() { }

    public Employee(EmployeeDto dto)
    {
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        MiddleName = dto.MiddleName;
        PassportNumber = dto.PassportNumber;
        DateOfBirth = dto.DateOfBirth;
        Nationality = dto.Nationality;
        Gender = dto.Gender;
        Pinfl = dto.Pinfl;
        PensionFundNumber = dto.PensionFundNumber;
        TaxIdentificationNumber = dto.TaxIdentificationNumber;
        PhoneNumber = dto.PhoneNumber;
        Address = dto.Address;
        HireDate = dto.HireDate;
        TerminationDate = dto.TerminationDate;
        DepartmentId = dto.DepartmentId;
        PositionId = dto.PositionId;
        Salary = dto.Salary;
        ScheduleId = dto.ScheduleId;
        Email = dto.Email;
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string MiddleName { get; private set; }

    public string PassportNumber { get; private set; }

    public DateOnly DateOfBirth { get; private set; }

    public string Nationality { get; private set; }

    public Gender Gender { get; private set; }

    public string Pinfl { get; private set; }

    public string PensionFundNumber { get; private set; }

    public string TaxIdentificationNumber { get; private set; }

    public string PhoneNumber { get; private set; }

    public Address Address { get; private set; }

    public DateOnly HireDate { get; private set; }

    public DateOnly? TerminationDate { get; private set; }

    public Guid DepartmentId { get; set; }

    public Department Department { get; private set; }

    public Guid PositionId { get; set; }

    public Position Position { get; private set; }

    public long Salary { get; private set; }

    public Guid ScheduleId { get; set; }

    public Schedule WorkSchedule { get; private set; }

    public string Email { get; private set; }
}
