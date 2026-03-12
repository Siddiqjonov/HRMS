using AutoMapper;
using HrManager.Application.UseCases.Schedules.CreateSchedule;
using HrManager.Application.UseCases.Schedules.UpdateSchedule;

namespace HrManager.Application.UseCases.Schedules;

public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<Schedule, ScheduleDto>();
        CreateMap<CreateScheduleRequest, Schedule>();
        CreateMap<UpdateScheduleRequest, Schedule>();
    }
}
