namespace FlightScanner.DTOs.Models;

public class AirportDto
{
    public AirportDto(string iataCode)
    {
        IataCode = iataCode;
    }

    public string IataCode { get; }
}
