using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestQueryOperations;

public class AbsenceRequestBriefInfo
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } 

    public RequestType Type { get; set; }

    public RequestStatus Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int DaysRequested { get; set; }

    public string? ManagerName { get; set; }
}
