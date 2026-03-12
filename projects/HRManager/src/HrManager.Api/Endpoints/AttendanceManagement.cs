using HrManager.Api.Infrastructure;
using HrManager.Application.UseCases.AttendanceManagement.CheckIn;
using HrManager.Application.UseCases.AttendanceManagement.CheckOut;
using HrManager.Application.UseCases.AttendanceManagement.Correction;
using HrManager.Application.UseCases.AttendanceManagement.GetAttendanceRecordsWithFilter;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HrManager.Api.Endpoints;

public class AttendanceManagement : EndpointGroupBase
{
    public override string? GroupName => "Attendance";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CheckIn, "/CheckIn")
            .RequireAuthorization(Policies.RequireEmployee);
        groupBuilder.MapPost(CheckOut, "/CheckOut")
            .RequireAuthorization(Policies.RequireEmployee);
        groupBuilder.MapPut(Correct, "/Correct")
            .RequireAuthorization(Policies.RequireHrManager);
        groupBuilder.MapGet(GetAttendanceRecords, "/Records")
            .RequireAuthorization(Policies.RequireEmployeeOrHrManager);
    }

    public static async Task<PaginatedList<GetAttendanceRecordsResponse>> GetAttendanceRecords(
    [AsParameters] GetAttendanceRecordsRequest request,
    ISender sender)
    {
        return await sender.Send(request);
    }

    public static async Task CheckIn(
        ISender sender, CheckInRequest request)
    {
        await sender.Send(request);
    }

    public static async Task CheckOut(
        ISender sender, CheckOutRequest request)
    {
        await sender.Send(request);
    }

    public static async Task<Results<Created, BadRequest>> Correct(
        ISender sender,
        [FromBody] CorrectAttendanceRecordRequest request)
    {
        var response = await sender.Send(request);

        if (response)
        {
            return TypedResults.Created();
        }

        return TypedResults.Created();
    }
}
