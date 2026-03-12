namespace HrManager.Application.Common.Exceptions.EmailExceptions;

public class EmailSendFailedException : Exception
{
    public EmailSendFailedException(string message)
        : base(message)
    {
    }

    public EmailSendFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
