namespace FlightScanner.Domain.Exceptions;

public class InvalidResponseException : Exception
{
    public InvalidResponseException(string? message) : base(message)
    {
    }
}
