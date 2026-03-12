namespace HrManager.Domain.Dtos;

public class AbsenceRequestDto
{
    public Guid EmployeeId { get; set; }

    public Guid ApproverId { get; set; }

    public RequestStatus RequestStatus { get; set; }

    public RequestType RequestType { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime? ProcessedAt { get; set; }
}
