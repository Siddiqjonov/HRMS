using HrManager.Application.Common.Services.EmailService;

namespace HrManager.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);

    Task SendEmailTemplateAsync(EmailTemplate template, CancellationToken cancellationToken = default);
}
