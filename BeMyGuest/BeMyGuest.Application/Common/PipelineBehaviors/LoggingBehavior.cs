using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BeMyGuest.Application.Common.PipelineBehaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopWatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName} request", requestName);

        try
        {
            var response = await next();

            _logger.LogInformation("Successfully handled {RequestName} request. Elapsed time: {ElapsedTime}", requestName, stopWatch.Elapsed);

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while handling {RequestName} request. Elapsed time: {ElapsedTime}", requestName, stopWatch.Elapsed);
            throw;
        }
    }
}