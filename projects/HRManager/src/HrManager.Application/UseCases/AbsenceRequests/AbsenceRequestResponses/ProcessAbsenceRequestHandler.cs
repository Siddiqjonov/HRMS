using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using HrManager.Application.Common.Services.EmailService.Templates;
using HrManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestResponses;

public class ProcessAbsenceRequestHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    IEmailService emailService,
    IAbsenceBalanceService balanceService) : IRequestHandler<ProcessAbsenceRequest, bool>
{
    public async Task<bool> Handle(ProcessAbsenceRequest request, CancellationToken cancellationToken)
    {
        var currentEmail = currentUser.Email;

        var entity = await context.AbsenceRequests
           .Include(r => r.Employee.Department.Manager)
           .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
           ?? throw new NotFoundException($"AbsenceRequest with Id {request.Id} not found.");

        if (!string.Equals(entity.Employee.Department.Manager.Email, currentEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException("Only the employee's department manager can process this request.");
        }

        if (request.Approved)
        {
            entity.Approve(entity.Employee.Department.Manager.Id, request.Reason);
            await balanceService.DeductBalanceAsync(entity.EmployeeId, entity.RequestType, entity.StartDate, entity.EndDate, cancellationToken);
        }
        else
        {
            entity.Reject(entity.Employee.Department.Manager.Id, request.Reason);
        }

        await context.SaveChangesAsync(cancellationToken);

        var absence = await context.AbsenceRequests
        .Where(r => r.Id == request.Id)
        .Select(r => new
        {
            EmployeeName = r.Employee.FirstName + " " + r.Employee.LastName,
            EmployeeEmail = r.Employee.Email,
            ManagerName = r.Employee.Department.Manager.FirstName + " " + r.Employee.Department.Manager.LastName,
            ManagerEmail = r.Employee.Department.Manager.Email,
            r.RequestType,
            Status = r.RequestStatus,
        })
        .FirstOrDefaultAsync(cancellationToken);

        var emailTemplate = absence.Status switch
        {
            RequestStatus.Approved => EmailTemplates.ManagerResponse(
                employeeName: absence.EmployeeName,
                requestType: absence.RequestType,
                status: RequestStatus.Approved,
                managerComment: request.Reason ?? "Your request has been approved.",
                managerName: absence.ManagerName,
                to: [absence.EmployeeEmail]),

            RequestStatus.Rejected => EmailTemplates.ManagerResponse(
                employeeName: absence.EmployeeName,
                requestType: absence.RequestType,
                status: RequestStatus.Rejected,
                managerComment: request.Reason ?? "Unfortunately, your request was rejected.",
                managerName: absence.ManagerName,
                to: [absence.EmployeeEmail]),
        };

        await emailService.SendEmailTemplateAsync(emailTemplate, cancellationToken);

        return true;
    }
}
