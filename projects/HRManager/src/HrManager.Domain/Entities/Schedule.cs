using HrManager.Domain.Dtos;

namespace HrManager.Domain.Entities;

public class Schedule : SoftDeletableAuditableEntity
{
    public Schedule() { }
   
    public Schedule(ScheduleDto dto)
    {
        Name = dto.Name;
        Description = dto.Description;
        StartTime = dto.StartTime;
        EndTime = dto.EndTime;
        DaysOfWeek = dto.DaysOfWeek;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public TimeOnly StartTime { get; private set; }

    public TimeOnly EndTime { get; private set; }

    public DaysOfWeek DaysOfWeek { get; private set; }
}
