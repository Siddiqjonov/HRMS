using HrManager.Domain.Dtos;

namespace HrManager.Domain.Entities;

public class AbsenceRequest : SoftDeletableAuditableEntity
{
    public AbsenceRequest() { }
   
    public AbsenceRequest(AbsenceRequestDto dto)
    {
        EmployeeId = dto.EmployeeId;
        RequestStatus = dto.RequestStatus;
        RequestType = dto.RequestType;
        StartDate = dto.StartDate;
        EndDate = dto.EndDate;
        Reason = dto.Reason;
        ApproverId = dto.ApproverId;
        ProcessedAt = dto.ProcessedAt;
    }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; private set; }

    public RequestStatus RequestStatus { get; private set; }

    public RequestType RequestType { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }
        
    public string? Reason { get; private set; }

    public Guid? ApproverId { get; set; }

    public Employee? Approver { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    public void Approve(Guid approverId, string? reason = null)
    {
        RequestStatus = RequestStatus.Approved;
        ApproverId = approverId;
        ProcessedAt = DateTime.UtcNow;
        Reason = reason ?? Reason;
    }

    public void Reject(Guid approverId, string? reason = null)
    {
        RequestStatus = RequestStatus.Rejected;
        ApproverId = approverId;
        ProcessedAt = DateTime.UtcNow;
        Reason = reason ?? Reason;
    }

}
