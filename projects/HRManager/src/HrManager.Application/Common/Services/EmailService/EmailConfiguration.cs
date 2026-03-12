using System.ComponentModel.DataAnnotations;

namespace HrManager.Application.Common.Services.EmailService;

public record EmailConfiguration
{
    [Required]
    public string SmtpServer { get; init; } 

    [Range(1, 65535)]
    public int Port { get; init; }

    [Required]
    public string SenderName { get; init; } 

    [Required]
    [EmailAddress]
    public string SenderEmail { get; init; } 

    [Required]
    public string UserName { get; init; } 

    [Required]
    public string Password { get; init; } 
}
