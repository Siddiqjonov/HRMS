using HrManager.Api.Infrastructure;
using HrManager.Application.UseCases.Employees;
using HrManager.Application.UseCases.Employees.CreateEmployee;
using HrManager.Application.UseCases.Employees.DeleteEmployee;
using HrManager.Application.UseCases.Employees.GetEmployee;
using HrManager.Application.UseCases.Employees.GetEmployeeByEmail;
using HrManager.Application.UseCases.Employees.GetEmployeeOverview;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Application.UseCases.Employees.UpdateEmployee;
using HrManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HrManager.Api.Endpoints;

public class Employees : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetEmployeesWithPagination, "/GetEmployeesWithPagination")
             .RequireAuthorization(Policies.RequireHrManager);

        groupBuilder.MapPost(CreateEmployee, "/CreateEmployee")
            .RequireAuthorization(Policies.RequireHrManager);

        groupBuilder.MapGet(GetEmployeeById, "/GetEmployeeById/{id:guid}")
            .RequireAuthorization(Policies.RequireAdminOrHrManager);

        groupBuilder.MapGet(GetEmployeeByEmail, "/GetEmployeeByEmail/{email}")
            .RequireAuthorization(Policies.RequireEmployee);

        groupBuilder.MapGet(GetEmployeeOverview, "/GetEmployeeOverview")
            .RequireAuthorization(Policies.RequireHrManager);

        groupBuilder.MapPut(UpdateEmployee, "/UpdateEmployee")
            .RequireAuthorization(Policies.RequireHrManager);

        groupBuilder.MapDelete(DeleteEmployee, "/DeleteEmployee/{id:guid}")
            .RequireAuthorization(Policies.RequireHrManager);
    }

    public static async Task<Ok<PaginatedList<EmployeesBriefResponse>>> GetEmployeesWithPagination(ISender sender, [AsParameters] GetEmployeesWithPaginationRequest request)
    {
        var result = await sender.Send(request);

        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<EmployeeResponse>, NotFound>> GetEmployeeById(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetEmployeeByIdRequest(id));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Ok<EmployeeResponse>, NotFound>> GetEmployeeByEmail(
       ISender sender,
       string email)
    {
        var result = await sender.Send(new GetEmployeeByEmailRequest(email));

        if (result is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound>> UpdateEmployee(ISender sender, UpdateEmployeeRequest request)
    {
        var result = await sender.Send(request);

        if (result)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Created, BadRequest>> CreateEmployee(
        ISender sender,
        CreateEmployeeRequest request)
    {
        var response = await sender.Send(request);

        if (response)
        {
            return TypedResults.Created();
        }

        return TypedResults.BadRequest();
    }

    public static async Task<NoContent> DeleteEmployee(ISender sender, Guid id)
    {
        await sender.Send(new DeleteEmployeeRequest(id));

        return TypedResults.NoContent();
    }

    public static async Task<Ok<EmployeeOverviewResponse>> GetEmployeeOverview(ISender sender)
    {
        var result = await sender.Send(new GetEmployeeOverviewRequest());
        return TypedResults.Ok(result);
    }
}
