using System.Net;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace FlightScanner.Common.Policies;

public static class HttpPoliciesFactory
{
    private const int MAX_RETRIES_COUNT = 3;
    private const int MAX_NUMBER_OF_CONSECUTIVE_TOO_MANY_REQUESTS = 2;
    private const int WAIT_AFTER_TOO_MANY_REQUESTS_IN_SECONDS = 10;

    public static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicyWithSameRetryTime(int seconds)
    {
        return Policy<HttpResponseMessage>
            .HandleResult(httpResponse =>
            {
                return httpResponse.StatusCode is not HttpStatusCode.TooManyRequests and not HttpStatusCode.OK;
            })
            .WaitAndRetryAsync(
                retryCount: MAX_RETRIES_COUNT,
                sleepDurationProvider: _ => TimeSpan.FromSeconds(seconds),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var errorReason = outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString();

                    var fullLog = $"{timespan}: Retry {retryCount} for {context.PolicyKey} at {context.OperationKey} due to: {errorReason}.";

                    Console.WriteLine(fullLog);
                });
    }

    public static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicyWithExponentialIncrementOfRetryTime()
    {
        return Policy<HttpResponseMessage>
            .HandleResult(httpResponse =>
            {
                return httpResponse.StatusCode is not HttpStatusCode.TooManyRequests and not HttpStatusCode.OK;
            })
            .WaitAndRetryAsync(
                retryCount: MAX_RETRIES_COUNT,
                sleepDurationProvider: counter =>
                {
                    var seconds = Math.Pow(2, counter);

                    return TimeSpan.FromSeconds(seconds);
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var errorReason = outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString();

                    var fullLog = $"{timespan}: Retry {retryCount} for {context.PolicyKey} at {context.OperationKey} due to: {errorReason}.";

                    Console.WriteLine(fullLog);
                });
    }

    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CreateCircuitBreakingPolicyForTooManyRequests()
    {
        return Policy<HttpResponseMessage>
            .HandleResult(httpResponse => httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: MAX_NUMBER_OF_CONSECUTIVE_TOO_MANY_REQUESTS,
                durationOfBreak: TimeSpan.FromSeconds(WAIT_AFTER_TOO_MANY_REQUESTS_IN_SECONDS),
                onBreak: (result, duration) =>
                {
                    Console.WriteLine($"Circuit opened for {duration.TotalSeconds} seconds.");
                },
                onReset: () => Console.WriteLine("Circuit closed, communication allowed."));
    }
}
