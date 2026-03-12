namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestResponses;

public class ProcessAbsenceRequest : IRequest<bool>
{
    public Guid Id { get; set; }

    public bool Approved { get; set; }

    public string? Reason { get; set; }
}
