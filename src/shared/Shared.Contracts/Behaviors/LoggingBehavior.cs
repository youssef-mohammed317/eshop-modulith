using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Shared.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("[START] Handling {RequestName}", requestName);

        var timer = Stopwatch.StartNew();
        var response = await next();
        timer.Stop();

        var timeTaken = timer.Elapsed;
        if (timeTaken.TotalSeconds > 3)
        {
            // Flags performance bottlenecks in long-running queries or database transactions
            _logger.LogWarning("[PERFORMANCE] {RequestName} took {TimeTaken} to execute.", requestName, timeTaken);
        }

        _logger.LogInformation("[END] Handled {RequestName}", requestName);

        return response;
    }
}