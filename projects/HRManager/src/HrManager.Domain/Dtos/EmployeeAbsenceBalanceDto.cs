namespace HrManager.Domain.Dtos;

public class EmployeeAbsenceBalanceDto
{
    public Guid EmployeeId { get; set; }

    public RequestType AbsenceType { get; set; }

    public int TotalDaysAllowed { get; set; }

    public int DaysUsed { get; set; }

    public DateOnly PeriodStartDate { get; set; }

    public DateOnly PeriodEndDate { get; set; }
}
