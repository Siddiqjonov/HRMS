using HrManager.Api.Infrastructure;
using HrManager.Application.UseCases.Departments.AssignDepartmentManager;
using HrManager.Application.UseCases.Departments.CreateDepartment;
using HrManager.Application.UseCases.Departments.DeleteDepartment;
using HrManager.Application.UseCases.Departments.GetDepartment;
using HrManager.Application.UseCases.Departments.GetDepartmentEmployees;
using HrManager.Application.UseCases.Departments.GetDepartmentOverview;
using HrManager.Application.UseCases.Departments.GetDepartmentsWithPagination;
using HrManager.Application.UseCases.Departments.RemoveDepartmentManager;
using HrManager.Application.UseCases.Departments.UpdateDepartment;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HrManager.Api.Endpoints;

public class Departments : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetDepartmentsWithPagination, "/GetDepartmentsWithPagination")
            .RequireAuthorization(Policies.RequireEmployeeOrHrManager);
        
        groupBuilder.MapPost(CreateDepartment, "/CreateDepartment")
            .RequireAuthorization(Policies.RequireHrManager);
        
        groupBuilder.MapPut(UpdateDepartment, "/UpdateDepartment")
            .RequireAuthorization(Policies.RequireHrManager);
        
        groupBuilder.MapDelete(DeleteDepartment, "/DeleteDepartment/{id:guid}")
            .RequireAuthorization(Policies.RequireHrManager);
        
        groupBuilder.MapGet(GetDepartmentById, "/GetDepartmentById/{id:guid}")
            .RequireAuthorization(Policies.RequireHrManager);
        
        groupBuilder.MapGet(GetDepartmentOverview, "/GetDepartmentOverview/{id:guid}")
            .RequireAuthorization(Policies.RequireEmployeeOrHrManager);
        
        groupBuilder.MapGet(GetDepartmentEmployees, "/GetDepartmentEmployees/{id:guid}")
            .RequireAuthorization(Policies.RequireEmployeeOrHrManager);
        
        groupBuilder.MapPost(AssignManager, "/AssignManager")
            .RequireAuthorization(Policies.RequireHrManager);
        
        groupBuilder.MapDelete(RemoveManager, "/RemoveManager/{id:guid}")
            .RequireAuthorization(Policies.RequireHrManager);
    }

    public static async Task<IResult> RemoveManager(
        ISender sender, Guid id)
    {
        var request = new RemoveDepartmentManagerRequest(id);
        await sender.Send(request);
        return Results.Ok();
    }

    public static async Task<IResult> AssignManager(
        ISender sender, AssignDepartmentManagerRequest request)
    {
        await sender.Send(request);
        return Results.Ok();
    }

    public static async Task<Results<Created, BadRequest>> CreateDepartment(
        ISender sender, CreateDepartmentRequest request)
    {
        var response = await sender.Send(request);

        if (response)
        {
            return TypedResults.Created();
        }

        return TypedResults.BadRequest();
    }

    public static async Task<NoContent> DeleteDepartment(ISender sender, Guid id)
    {
        await sender.Send(new DeleteDepartmentRequest(id));
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> UpdateDepartment(
        ISender sender, UpdateDepartmentRequest request)
    {
        var result = await sender.Send(request);

        if (result)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }

    public static async Task<Ok<PaginatedList<DepartmentResponse>>> GetDepartmentsWithPagination(ISender sender, [AsParameters] GetDepartmentsWithPaginationRequest request)
    {
        var result = await sender.Send(request);
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<DepartmentDetailsResponse>, NotFound>> GetDepartmentById(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetDepartmentByIdRequest(id));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Ok<DepartmentOverviewResponse>, NotFound>> GetDepartmentOverview(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetDepartmentOverviewRequest(id));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Ok<List<EmployeesBriefResponse>>> GetDepartmentEmployees(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetDepartmentEmployeesRequest(id));
        return TypedResults.Ok(result);
    }
}