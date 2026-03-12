using HrManager.Application.Common.Services;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;
using HrManager.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestQueryOperations;

public class GetAbsenceRequestsWithPaginationRequestHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetAbsenceRequestsWithPaginationRequest, PaginatedList<AbsenceRequestBriefInfo>>
{
    public async Task<PaginatedList<AbsenceRequestBriefInfo>> Handle(GetAbsenceRequestsWithPaginationRequest request, CancellationToken cancellationToken)
    {
        var query = context.AbsenceRequests.AsNoTracking();

        var userEmail = currentUser.Email;
        var roles = currentUser.UserRoles;
        
        if (!roles.Contains(Roles.Admin) && !roles.Contains(Roles.HrManager))
        {
            var currentEmployee = await context.Employees
                .AsNoTracking()
                .Where(e => e.Email == userEmail)
                .Select(e => new { e.Id, IsManager = context.Departments.Any(d => d.ManagerId == e.Id) })
                .FirstOrDefaultAsync(cancellationToken);

            if (currentEmployee != null)
            {
                if (currentEmployee.IsManager)
                {
                    query = query.Where(r => r.ApproverId == currentEmployee.Id || r.Employee.Email == userEmail);
                }
                else
                {
                    query = query.Where(r => r.Employee.Email == userEmail);
                }
            }
            else
            {
                query = query.Where(r => false);
            }
        }

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(r => r.EmployeeId == request.EmployeeId.Value);
        }

        if (request.ManagerId.HasValue)
        {
            query = query.Where(r => r.ApproverId == request.ManagerId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.RequestStatus == request.Status.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(r => r.RequestType == request.Type.Value);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(r => r.StartDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(r => r.EndDate <= request.EndDate.Value);
        }

        var dtoQuery = query
           .OrderByDescending(r => r.StartDate)
           .Select(r => new AbsenceRequestBriefInfo
           {
               Id = r.Id,
               EmployeeId = r.EmployeeId,
               EmployeeName = r.Employee.FirstName + " " + r.Employee.LastName,
               ManagerName = r.Approver != null ? r.Approver.FirstName + " " + r.Approver.LastName : string.Empty,
               Type = r.RequestType,
               Status = r.RequestStatus,
               StartDate = r.StartDate,
               EndDate = r.EndDate,
               DaysRequested = r.EndDate.DayNumber - r.StartDate.DayNumber + 1,
           });

        var pagedResult = await PaginatedList<AbsenceRequestBriefInfo>.CreateAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return pagedResult;
    }
}
