using HrManager.Api.Infrastructure;
using HrManager.Application.UseCases.EmployeeDocuments.DeleteEmployeeDocument;
using HrManager.Application.UseCases.EmployeeDocuments.GetDocumentDownloadUrl;
using HrManager.Application.UseCases.EmployeeDocuments.GetEmployeeDocuments;
using HrManager.Application.UseCases.EmployeeDocuments.UploadEmployeeDocument;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HrManager.Api.Endpoints;

public class EmployeeDocuments : EndpointGroupBase
{
    public override string? GroupName { get; } = "EmployeeDocuments";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(UploadDocument, "/UploadDocument")
         .RequireAuthorization(Policies.RequireHrManager)
         .DisableAntiforgery();

        groupBuilder.MapGet(GetDocuments, "/GetDocuments")
         .RequireAuthorization(Policies.RequireHrManager);
         
        groupBuilder.MapGet(GetDownloadUrl, "/{documentId:guid}/GetDownloadUrl")
          .RequireAuthorization(Policies.RequireHrManager);
          
        groupBuilder.MapDelete(DeleteDocument, "/{documentId:guid}/DeleteDocument")
          .RequireAuthorization(Policies.RequireHrManager);
    }

    public static async Task<Created<EmployeeDocumentsResponse>> UploadDocument(
        ISender sender,
        [FromForm] UploadEmployeeDocumentRequest request)
    {
        var response = await sender.Send(request);
        return TypedResults.Created($"/api/documents/{response.id}", response);
    }

    public static async Task<Ok<PaginatedList<EmployeeDocumentsResponse>>> GetDocuments(
        ISender sender,
        [AsParameters] GetEmployeeDocumentsRequest request)
    {
        var result = await sender.Send(request);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<GetDocumentDownloadUrlResponse>> GetDownloadUrl(
        ISender sender,
        [FromRoute] Guid documentId)
    {
        var result = await sender.Send(new GetDocumentDownloadUrlRequest(documentId));
        return TypedResults.Ok(result);
    }

    public static async Task<NoContent> DeleteDocument(
        ISender sender,
        [FromRoute] Guid documentId)
    {
        await sender.Send(new DeleteEmployeeDocumentRequest(documentId));
        return TypedResults.NoContent();
    }
}
