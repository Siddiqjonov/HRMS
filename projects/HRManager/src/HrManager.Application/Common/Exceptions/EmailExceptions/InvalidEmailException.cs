namespace HrManager.Application.Common.Exceptions.EmailExceptions;

public class InvalidEmailException(string message) : Exception(message)
{
}
