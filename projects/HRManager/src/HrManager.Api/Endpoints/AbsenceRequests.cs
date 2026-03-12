using HrManager.Api.Infrastructure;
using HrManager.Application.UseCases.AbsenceRequests;
using HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestQueryOperations;
using HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestResponses;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HrManager.Api.Endpoints;

public class AbsenceRequests : EndpointGroupBase
{
    public override string? GroupName => "AbsenceRequests";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization(Policies.RequireEmployee);

        groupBuilder.MapPost(CreateAbsenceRequest, "/CreateAbsenceRequest");
        groupBuilder.MapPut(ProcessAbsenceRequest, "/ProcessAbsenceRequest");
        groupBuilder.MapGet(GetAbsenceRequestsWithPagination, "/GetAbsenceRequestsWithPagination");
    }

    public static async Task<Ok<PaginatedList<AbsenceRequestBriefInfo>>> GetAbsenceRequestsWithPagination([AsParameters] GetAbsenceRequestsWithPaginationRequest request, ISender sender)
    {
        var response = await sender.Send(request);

        return TypedResults.Ok(response);
    }

    public static async Task<Results<Created, BadRequest>> CreateAbsenceRequest(
        ISender sender,
        CreateAbsenceRequestRequest request)
    {
        var response = await sender.Send(request);

        if (!response)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Created();
    }

    public static async Task<Results<NoContent, NotFound>> ProcessAbsenceRequest(
        ISender sender,
        ProcessAbsenceRequest request)
    {
        var result = await sender.Send(request);

        if (result)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}
