using HrManager.Api.Infrastructure;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Application.UseCases.Schedules;
using HrManager.Application.UseCases.Schedules.CreateSchedule;
using HrManager.Application.UseCases.Schedules.DeleteSchedule;
using HrManager.Application.UseCases.Schedules.GetScheduleById;
using HrManager.Application.UseCases.Schedules.GetScheduleEmployees;
using HrManager.Application.UseCases.Schedules.GetSchedulesWithPagination;
using HrManager.Application.UseCases.Schedules.UpdateSchedule;
using HrManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HrManager.Api.Endpoints;

public class Schedule : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization(Policies.RequireHrManager);
        groupBuilder.MapGet(GetScheduleById, "/GetScheduleById/{id:guid}");
        groupBuilder.MapPost(CreateSchedule, "/CreateSchedule");
        groupBuilder.MapPut(UpdateSchedule, "/UpdateSchedule");
        groupBuilder.MapDelete(DeleteSchedule, "/DeleteSchedule/{id:guid}");
        groupBuilder.MapGet(GetSchedulesWithPagination, "/GetSchedulesWithPagination");
        groupBuilder.MapGet(GetScheduleEmployees, "/GetScheduleEmployees/{scheduleId:guid}");
    }

    public static async Task<Results<Ok<ScheduleDto>, NotFound>> GetScheduleById(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetScheduleByIdRequest(id));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Created<ScheduleDto>> CreateSchedule(ISender sender, CreateScheduleRequest request)
    {
        var response = await sender.Send(request);

        return TypedResults.Created($"/api/schedules/{response.Id}", response);
    }

    public static async Task<Results<NoContent, NotFound>> UpdateSchedule(ISender sender, UpdateScheduleRequest request)
    {
        var result = await sender.Send(request);

        if (result)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }

    public static async Task<NoContent> DeleteSchedule(ISender sender, Guid id)
    {
        await sender.Send(new DeleteScheduleRequest(id));

        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<PaginatedList<ScheduleDto>>, NotFound>> GetSchedulesWithPagination(ISender sender, int pageNumber = 1, int pageSize = 10)
    {
        var result = await sender.Send(new GetSchedulesWithPaginationRequest(pageNumber, pageSize));

        if (result != null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Ok<IEnumerable<EmployeesBriefResponse>>, NotFound>> GetScheduleEmployees(ISender sender, Guid scheduleId)
    {
        var result = await sender.Send(new GetScheduleEmployeesRequest(scheduleId));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }
}
