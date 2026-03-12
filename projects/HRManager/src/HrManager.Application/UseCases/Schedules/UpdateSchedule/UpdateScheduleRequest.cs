using HrManager.Domain.Enums;

namespace HrManager.Application.UseCases.Schedules.UpdateSchedule;

public record UpdateScheduleRequest(
    Guid Id,
    string Name,
    string Description,
    TimeOnly StartTime,
    TimeOnly EndTime,
    DaysOfWeek DaysOfWeek)
    : IRequest<bool>;
