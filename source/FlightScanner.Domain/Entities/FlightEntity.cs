namespace FlightScanner.Domain.Entities;

public class FlightEntity
{
    public FlightEntity(
        string departureAirport,
        string departureAirportIataCode,
        string departureDate,
        string arrivalAirport,
        string arrivalAirportIataCode,
        string arrivalDate)
    {
        DepartureAirport = departureAirport;
        DepartureAirportIataCode = departureAirportIataCode;
        DepartureDate = departureDate;
        ArrivalAirport = arrivalAirport;
        ArrivalAirportIataCode = arrivalAirportIataCode;
        ArrivalDate = arrivalDate;
    }

    public string DepartureAirport { get; }

    public string DepartureAirportIataCode { get; }

    public string DepartureDate { get; }

    public string ArrivalAirport { get; }

    public string ArrivalAirportIataCode { get; }

    public string ArrivalDate { get; }
}
