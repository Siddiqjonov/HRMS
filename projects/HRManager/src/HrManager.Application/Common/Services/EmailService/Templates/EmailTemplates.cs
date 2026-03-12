using HrManager.Domain.Enums;

namespace HrManager.Application.Common.Services.EmailService.Templates;

public static class EmailTemplates
{
    public static EmailTemplate Welcome(string firstName, string email, List<string> to)
    => new EmailTemplate
    {
        TemplateName = "Welcome",
        Subject = "Welcome to HR Manager!",
        Body = $@"
            <h1>Hello {{FirstName}},</h1>
            <p>Welcome to the company! Your email is {{Email}}.</p>
            <p>Best regards,<br/>HR Team</p>
        ",
        To = to,
        Placeholders = new Dictionary<string, string>
        {
            { "FirstName", firstName },
            { "Email", email },
        },
    };

    public static EmailTemplate PasswordReset(string firstName, string resetLink, List<string> to)
    => new EmailTemplate
    {
        TemplateName = "PasswordReset",
        Subject = "Password Reset Instructions",
        Body = $@"
            <h1>Hello {{FirstName}},</h1>
            <p>Click the link below to reset your password:</p>
            <p><a href='{{ResetLink}}'>Reset Password</a></p>
            <p>Best regards,<br/>HR Team</p>
        ",
        To = to,
        Placeholders = new Dictionary<string, string>
        {
            { "FirstName", firstName },
            { "ResetLink", resetLink },
        },
    };

    public static EmailTemplate RequestCreated(
    string employeeName,
    RequestType requestType,
    DateOnly fromDate,
    DateOnly toDate,
    string reason,                    
    List<string> to)
    => new EmailTemplate
    {
        TemplateName = "RequestCreated",
        Subject = $"New {requestType} Request Submitted",
        Body = $@"
            <h1>Hello,</h1>
            <p>Employee <b>{{EmployeeName}}</b> has submitted a new <b>{{RequestType}}</b> request.</p>
            <p><b>From:</b> {{FromDate}} <br/> <b>To:</b> {{ToDate}}</p>
            <p><b>Reason:</b> {{Reason}}</p>     
            <p>Status: <b>Pending</b></p>
            <p>Best regards,<br/>HR Team</p>
        ",
        To = to,
        Placeholders = new Dictionary<string, string>
        {
            { "EmployeeName", employeeName },
            { "RequestType", requestType.ToString() },
            { "FromDate", fromDate.ToShortDateString() },
            { "ToDate", toDate.ToShortDateString() },
            { "Reason", reason },
        },
    };


    public static EmailTemplate RequestCreatedForEmployee(
    string firstName,
    RequestType requestType,
    DateOnly fromDate,
    DateOnly toDate,
    List<string> to)
    => new EmailTemplate
    {
        TemplateName = "RequestCreatedForEmployee",
        Subject = $"{requestType} Request Submitted Successfully",
        Body = $@"
            <h1>Hello {{FirstName}},</h1>
            <p>Your <b>{{RequestType}}</b> request has been submitted successfully.</p>
            <p><b>From:</b> {{FromDate}} <br/> <b>To:</b> {{ToDate}}</p>
            <p>Status: <b>Pending</b></p>
            <p>You will be notified once your manager reviews it.</p>
            <p>Best regards,<br/>HR Team</p>
        ",
        To = to,
        Placeholders = new Dictionary<string, string>
        {
            { "FirstName", firstName },
            { "RequestType", requestType.ToString() },
            { "FromDate", fromDate.ToShortDateString() },
            { "ToDate", toDate.ToShortDateString() },
        },
    };

    public static EmailTemplate RequestStatusUpdate(
        string firstName,
        RequestType requestType,
        RequestStatus status,
        string managerName,
        List<string> to)
        => new EmailTemplate
        {
            TemplateName = "RequestStatusUpdate",
            Subject = $"{requestType} Request {status}",
            Body = $@"
                <h1>Hello {{FirstName}},</h1>
                <p>Your {{RequestType}} request has been <b>{{Status}}</b>.</p>
                <p>Reviewed by: {{ManagerName}}</p>
                <p>Best regards,<br/>HR Team</p>
            ",
            To = to,
            Placeholders = new Dictionary<string, string>
            {
                { "FirstName", firstName },
                { "RequestType", requestType.ToString() },
                { "Status", status.ToString() },
                { "ManagerName", managerName },
            },
        };

    public static EmailTemplate ConsecutiveLateArrivalWarning(
    string firstName,
    int lateCount,
    List<string> to)
    => new EmailTemplate
    {
        TemplateName = "ConsecutiveLate",
        Subject = "Repeated Late Arrival Notice",
        Body = $@"
            <h1>Hello {{FirstName}},</h1>
            <p>Our records show that you have been late <b>{{LateCount}}</b> times consecutively.</p>
            <p>Please ensure punctual attendance to avoid further action.</p>
            <p>Best regards,<br/>HR Team</p>
        ",
        To = to,
        Placeholders = new Dictionary<string, string>
        {
            { "FirstName", firstName },
            { "LateCount", lateCount.ToString() },
        },
    };

    public static EmailTemplate ManagerResponse(
      string employeeName,
      RequestType requestType,
      RequestStatus status,
      string? managerComment,
      string managerName,
      List<string> to)
      => new EmailTemplate
      {
          TemplateName = "ManagerResponse",
          Subject = $"{requestType} Request {status}",
          Body = $@"
            <h1>Hello {{EmployeeName}},</h1>
            <p>Your <b>{{RequestType}}</b> request has been <b>{{Status}}</b>.</p>
            {{ManagerComment}}
            <p>Best regards,<br/>{{ManagerName}}</p>
        ",
          To = to,
          Placeholders = new Dictionary<string, string>
          {
            { "EmployeeName", employeeName },
            { "RequestType", requestType.ToString() },
            { "Status", status.ToString() },
            { "ManagerName", managerName },
            { "ManagerComment", string.IsNullOrWhiteSpace(managerComment)
                ? ""
                : $"<p><b>Manager's Comment:</b> {managerComment}</p>" },
          },
      };
}
