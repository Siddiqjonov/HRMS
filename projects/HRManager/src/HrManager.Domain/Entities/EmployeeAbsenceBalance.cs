using HrManager.Domain.Dtos;

namespace HrManager.Domain.Entities;

public class EmployeeAbsenceBalance : SoftDeletableAuditableEntity
{
    public EmployeeAbsenceBalance() { }
   
    public EmployeeAbsenceBalance(EmployeeAbsenceBalanceDto dto)
    {
        EmployeeId = dto.EmployeeId;
        AbsenceType = dto.AbsenceType;
        TotalDaysAllowed = dto.TotalDaysAllowed;
        DaysUsed = dto.DaysUsed;
        PeriodStartDate = dto.PeriodStartDate;
        PeriodEndDate = dto.PeriodEndDate;
    }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; private set; }

    public RequestType AbsenceType { get; private set; }

    public int TotalDaysAllowed { get; private set; }

    public int DaysUsed { get; private set; }

    public DateOnly PeriodStartDate { get; private set; }

    public DateOnly PeriodEndDate { get; private set; }

    public void UseDays(int days)
    {
        DaysUsed += days;
    }
}
