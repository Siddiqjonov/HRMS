namespace HrManager.Application.Common.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow.AddHours(5);
}