namespace FlightScanner.AirportCodes.Cli.Models;

public record Airport
{
    public Airport(string iATACode, string airportName, string location)
    {
        IATACode = iATACode;
        AirportName = airportName;
        Location = location;
    }

    public string IATACode { get; }

    public string AirportName { get; }

    public string Location { get; }
}
