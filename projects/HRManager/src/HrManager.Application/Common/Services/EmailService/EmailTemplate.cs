namespace HrManager.Application.Common.Services.EmailService;

public class EmailTemplate
{
    public string TemplateName { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public Dictionary<string, string> Placeholders { get; set; }

    public List<string> To { get; set; }

    public List<EmailAttachment> Attachments { get; set; }
}
