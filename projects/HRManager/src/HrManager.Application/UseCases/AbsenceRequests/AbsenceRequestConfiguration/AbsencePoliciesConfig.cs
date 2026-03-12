namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestConfiguration;

public class AbsencePoliciesConfig
{
    public AbsencePolicy Vacation { get; set; } = new AbsencePolicy();

    public AbsencePolicy Remote { get; set; } = new AbsencePolicy();
}
