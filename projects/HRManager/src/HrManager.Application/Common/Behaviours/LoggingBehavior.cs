using HrManager.Application.Common.Services;
using Microsoft.Extensions.Logging;

namespace HrManager.Application.Common.Behaviours;

public class LoggingBehavior<TRequest, TResponse>(IDateTimeService dateTimeService, ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;

        logger.LogInformation("Starting Request: {RequestName} at {DateTime}", requestName, dateTimeService.UtcNow);

        var response = await next(cancellationToken);

        logger.LogInformation("Completed Request: {RequestName} at {DateTime}", requestName, dateTimeService.UtcNow);

        return response;
    }
}
