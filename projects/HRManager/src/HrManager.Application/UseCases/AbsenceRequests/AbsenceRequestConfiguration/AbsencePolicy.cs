using System.ComponentModel.DataAnnotations;

namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestConfiguration;

public class AbsencePolicy
{
    [Range(1, int.MaxValue, ErrorMessage = "DaysAllowed must be greater than 0.")]

    public int DaysAllowed { get; set; }

    [RegularExpression("Year|Month", ErrorMessage = "Period must be either 'Year' or 'Month'.")]
    public string Period { get; set; }
}
