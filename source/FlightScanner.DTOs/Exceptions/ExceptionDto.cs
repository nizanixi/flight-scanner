namespace FlightScanner.DTOs.Exceptions;

public class ExceptionDto
{
    public ExceptionDto(string title, string exceptionMessage, string stackTrace, string? innerExceptionStackTrace)
    {
        Title = title;
        ExceptionMessage = exceptionMessage;
        StackTrace = stackTrace;
        InnerExceptionStackTrace = innerExceptionStackTrace;
    }

    public string Title { get; }

    public string ExceptionMessage { get; }

    public string StackTrace { get; }

    public string? InnerExceptionStackTrace { get; }
}
