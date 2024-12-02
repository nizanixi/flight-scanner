namespace FlightScanner.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(Type typeName, string objectId)
        : base($"{typeName} with id {objectId} was not found!")
    {
    }
}
