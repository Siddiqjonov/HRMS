namespace HrManager.Application.Common.Services.EmailService;

public class EmailMessage
{
    public List<string> To { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public List<EmailAttachment>? Attachments { get; set; }

    public bool IsHtml { get; set; }
}   
