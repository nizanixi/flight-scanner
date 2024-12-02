namespace FlightScanner.Domain.Models;

public class Airport
{
    public Airport(string iataCode, string? airportName, string? location)
    {
        IataCode = iataCode;
        AirportName = airportName;
        Location = location;
    }

    public string IataCode { get; }

    public string? AirportName { get; }

    public string? Location { get; }
}
