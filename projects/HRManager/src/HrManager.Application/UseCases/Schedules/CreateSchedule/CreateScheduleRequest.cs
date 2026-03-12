using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.Schedules.CreateSchedule;

public record CreateScheduleRequest(
    string Name,
    string Description,
    TimeOnly StartTime,
    TimeOnly EndTime,
    DaysOfWeek DaysOfWeek
) : IRequest<ScheduleDto>;
