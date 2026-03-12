using HrManager.Api.Infrastructure;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Application.UseCases.Positions;
using HrManager.Application.UseCases.Positions.CreatePosition;
using HrManager.Application.UseCases.Positions.DeletePosition;
using HrManager.Application.UseCases.Positions.GetPositionById;
using HrManager.Application.UseCases.Positions.GetPositionEmployees;
using HrManager.Application.UseCases.Positions.GetPositionsByDeparmentId;
using HrManager.Application.UseCases.Positions.GetPositionsWithPagination;
using HrManager.Application.UseCases.Positions.UpdatePosition;
using HrManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HrManager.Api.Endpoints;

public class Position : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization(Policies.RequireHrManager);
        groupBuilder.MapGet(GetPositionById, "/GetPositionById/{id:guid}");
        groupBuilder.MapPost(CreatePosition, "/CreatePosition");
        groupBuilder.MapPut(UpdatePosition, "/UpdatePosition");
        groupBuilder.MapDelete(DeletePosition, "/DeletePosition/{id:guid}");
        groupBuilder.MapGet(GetPositionsByDepartmentId, "/GetPositionsByDepartmentId/{departmentId:guid}");
        groupBuilder.MapGet(GetPositionEmployees, "/GetPositionEmployees/{positionId:guid}");
        groupBuilder.MapGet(GetPositionsWithPagination, "/GetPositionsWithPagination");

    }

    public static async Task<Results<Ok<PositionDto>, NotFound>> GetPositionById(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetPositionById(id));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Created, BadRequest>> CreatePosition(ISender sender, CreatePositionRequest request)
    {
        var response = await sender.Send(request);

        if (response)
        {
            return TypedResults.Created();
        }

        return TypedResults.Created(); 
    }

    public static async Task<Results<NoContent, NotFound>> UpdatePosition(ISender sender, UpdatePositionRequest request)
    {
        var result = await sender.Send(request);

        if (result)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }

    public static async Task<NoContent> DeletePosition(ISender sender, Guid id)
    {
        await sender.Send(new DeletePositionRequest(id));

        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<IEnumerable<PositionDto>>, NotFound>> GetPositionsByDepartmentId(ISender sender, Guid departmentId)
    {
        var result = await sender.Send(new GetPositionsByDepartmentIdRequest(departmentId));

        if (result is not null && result.Any())
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Ok<IEnumerable<EmployeesBriefResponse>>, NotFound>> GetPositionEmployees(ISender sender, Guid positionId)
    {
        var result = await sender.Send(new GetPositionEmployeesRequest(positionId));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Ok<PaginatedList<PositionDto>>, NotFound>> GetPositionsWithPagination(ISender sender, int pageNumber = 1, int pageSize = 10)
    {
        var result = await sender.Send(new GetPositionsWithPaginationRequest(pageNumber: pageNumber, pageSize: pageSize));

        if (result != null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }
}
