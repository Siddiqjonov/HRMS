namespace HrManager.Application.UseCases.Schedules.GetScheduleById;

public record GetScheduleByIdRequest(Guid Id) : IRequest<ScheduleDto>;
