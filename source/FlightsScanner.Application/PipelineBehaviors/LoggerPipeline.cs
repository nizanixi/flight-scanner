using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightsScanner.Application.PipelineBehaviors;

public class LoggerPipeline<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _stopWatch;
    private readonly ILogger<LoggerPipeline<TRequest, TResponse>> _logger;

    public LoggerPipeline(ILogger<LoggerPipeline<TRequest, TResponse>> logger)
    {
        _stopWatch = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _stopWatch.Start();

        var requestName = typeof(TRequest).Name;
        var requestParameters = request.ToString() ?? string.Empty;
        _logger.LogInformation(
            message: "Executing request {0} with parameters {1}.",
            args: [requestName, requestParameters]
            );

        var response = await next();

        _stopWatch.Stop();

        var duration = _stopWatch.Elapsed;
        var responseParameters = response?.ToString() ?? string.Empty;

        _logger.LogInformation(
            message: "Request {0} with parameters {1} completed after {2} with response {3}",
            args: [requestName, requestParameters, duration, responseParameters]
        );

        _stopWatch.Reset();

        return response;
    }
}
