using HrManager.Application.Common.Exceptions.EmailExceptions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;

namespace HrManager.Application.Common.Services.EmailService;

public class EmailService(IOptions<EmailConfiguration> options) : IEmailService
{
    private readonly EmailConfiguration options = options.Value;

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var validEmails = GetValidEmailAddresses(message.To);

        if (validEmails.Count == 0)
        {
            throw new InvalidEmailException("No valid recipients found for this email.");
        }

        message.To = validEmails;

        try
        {
            var mimeMessage = BuildMimeMessage(message);

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(options.SmtpServer, options.Port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(options.UserName, options.Password, cancellationToken);
            await smtp.SendAsync(mimeMessage, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (SmtpCommandException ex)
        {
            throw new EmailSendFailedException("SMTP command failed", ex);
        }
    }

    public async Task SendEmailTemplateAsync(EmailTemplate template, CancellationToken cancellationToken)
    {
        var bodyBuilder = new StringBuilder(template.Body);

        foreach (var placeholder in template.Placeholders)
        {
            bodyBuilder.Replace($"{{{placeholder.Key}}}", placeholder.Value);
        }

        var message = new EmailMessage
        {
            To = template.To,
            Subject = template.Subject,
            Body = bodyBuilder.ToString(),
            IsHtml = true,
            Attachments = template.Attachments,
        };

        await SendEmailAsync(message, cancellationToken);
    }

    private List<string> GetValidEmailAddresses(IEnumerable<string> recipients)
    {
        var validEmails = new List<string>();
        foreach (var recipient in recipients)
        {
            if (MailboxAddress.TryParse(recipient, out _))
            {
                validEmails.Add(recipient);
            }
        }

        return validEmails;
    }

    private MimeMessage BuildMimeMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(new MailboxAddress(options.SenderName, options.SenderEmail));

        foreach (var recipient in message.To)
        {
            mimeMessage.To.Add(MailboxAddress.Parse(recipient));
        }

        mimeMessage.Subject = message.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = message.IsHtml ? message.Body : null,
            TextBody = !message.IsHtml ? message.Body : null,
        };

        if (message.Attachments is not null)
        {
            foreach (var attachment in message.Attachments)
            {
                builder.Attachments.Add(
                    attachment.FileName,
                    attachment.Content,
                    ContentType.Parse(attachment.ContentType));
            }
        }

        mimeMessage.Body = builder.ToMessageBody();
        return mimeMessage;
    }
}
