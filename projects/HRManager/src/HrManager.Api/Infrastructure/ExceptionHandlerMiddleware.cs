using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Exceptions.EmailExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HrManager.Api.Infrastructure;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

    public ExceptionHandlerMiddleware()
    {
        _exceptionHandlers = new ()
        {
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(BadRequestException), HandleBadRequestException },
            { typeof(ConflictException), HandleConflictException },
            { typeof(ValidationException), HandleValidationException },
            { typeof(InvalidEmailException),HandleInvalidEmailException},
            { typeof(EmailSendFailedException),HandleEmailSendFailedException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
        };
    }

    public async ValueTask<bool> TryHandleAsync(
         HttpContext httpContext,
         Exception exception,
         CancellationToken cancellationToken)
    {
        if (_exceptionHandlers.TryGetValue(exception.GetType(), out var handler))
        {
            await handler(httpContext, exception);
            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        });

        return true;
    }

    private async Task HandleNotFoundException(HttpContext context, Exception ex)
    {
        var exception = (NotFoundException)ex;

        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
        });
    }

    private async Task HandleBadRequestException(HttpContext context, Exception ex)
    {
        var exception = (BadRequestException)ex;

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.2",
        });
    }

    private async Task HandleConflictException(HttpContext context, Exception ex)
    {
        var exception = (ConflictException)ex;

        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
        });
    }

    private async Task HandleValidationException(HttpContext context, Exception ex)
    {
        var exception = (ValidationException)ex;

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.2",
        });
    }

    private async Task HandleInvalidEmailException(HttpContext context, Exception ex)
    {
        var exception = (InvalidEmailException)ex;

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.2",
        });
    }

    private async Task HandleEmailSendFailedException(HttpContext context, Exception ex)
    {
        var exception = (EmailSendFailedException)ex;

        context.Response.StatusCode = StatusCodes.Status502BadGateway;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status502BadGateway,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.3",
        });
    }

    private async Task HandleForbiddenAccessException(HttpContext context, Exception ex)
    {

        var exception = (ForbiddenAccessException)ex;
        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        });
    }
}
