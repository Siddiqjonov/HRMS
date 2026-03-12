using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.AbsenceRequests;

public class CreateAbsenceRequestRequest : IRequest<bool>
{
    public Guid EmployeeId { get; set; }

    public RequestType RequestType { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }
}