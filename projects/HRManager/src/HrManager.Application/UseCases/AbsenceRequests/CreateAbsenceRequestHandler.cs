using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using HrManager.Application.Common.Services.EmailService.Templates;
using HrManager.Domain.Dtos;
using HrManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.AbsenceRequests;

public class CreateAbsenceRequestHandler(
    IApplicationDbContext context,
    IDateTimeService dateTimeService,
    IEmailService emailService) : IRequestHandler<CreateAbsenceRequestRequest, bool>
{
    public async Task<bool> Handle(CreateAbsenceRequestRequest request, CancellationToken cancellationToken)
    {
        var employeeData = await context.Employees
            .Include(e => e.Department)
                .ThenInclude(d => d.Manager)
            .Where(e => e.Id == request.EmployeeId)
            .Select(e => new
            {
                e.FirstName,
                EmployeeName = e.FirstName + " " + e.LastName,
                EmployeeEmail = e.Email,
                ManagerEmail = e.Department.Manager != null ? e.Department.Manager.Email : null,
                e.Department.ManagerId,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Employee with ID '{request.EmployeeId}' was not found.");

        if (employeeData.ManagerId == null)
        {
            throw new NotFoundException("You cannot create an absence request because your department does not have a manager assigned.");
        }

        var absenceRequest = new AbsenceRequest(new AbsenceRequestDto
        {
            EmployeeId = request.EmployeeId,
            RequestType = request.RequestType,
            RequestStatus = RequestStatus.Pending,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason ?? string.Empty,
            ApproverId = (Guid)employeeData.ManagerId,
            ProcessedAt = dateTimeService.UtcNow,
        });

        await context.AbsenceRequests.AddAsync(absenceRequest, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(employeeData.ManagerEmail))
        {
            var email = EmailTemplates.RequestCreated(
                employeeName: employeeData.EmployeeName,
                requestType: request.RequestType,
                fromDate: request.StartDate,
                toDate: request.EndDate,
                reason: request.Reason ?? string.Empty,
                to: [employeeData.ManagerEmail]);

            await emailService.SendEmailTemplateAsync(email, cancellationToken);
        }

        if (!string.IsNullOrEmpty(employeeData.EmployeeEmail))
        {
            var employeeEmail = EmailTemplates.RequestCreatedForEmployee(
                firstName: employeeData.FirstName,
                requestType: request.RequestType,
                fromDate: request.StartDate,
                toDate: request.EndDate,
                to: [employeeData.EmployeeEmail]);

            await emailService.SendEmailTemplateAsync(employeeEmail, cancellationToken);
        }

        return true;
    }
}