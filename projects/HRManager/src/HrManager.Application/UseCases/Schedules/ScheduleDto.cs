using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.Schedules;

public class ScheduleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DaysOfWeek DaysOfWeek { get; set; }
}
