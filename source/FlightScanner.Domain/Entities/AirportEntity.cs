namespace FlightScanner.Domain.Entities;

public class AirportEntity
{
    public AirportEntity(string iataCode, string? airportName, string? location)
    {
        IataCode = iataCode;
        AirportName = airportName;
        Location = location;
    }

    public string IataCode { get; }

    public string? AirportName { get; }

    public string? Location { get; }
}
