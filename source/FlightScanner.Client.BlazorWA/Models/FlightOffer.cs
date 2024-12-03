namespace FlightScanner.Client.BlazorWA.Models;

public class FlightOffer
{
    public FlightOffer(
        string departureAirportIataCode,
        DateTime departureDate,
        string arrivalAirportIataCode,
        DateTime arrivalDate,
        uint numberOfBookableSeats,
        int numberOfStops,
        string currency,
        double price)
    {
        DepartureAirportIataCode = departureAirportIataCode;
        DepartureDate = departureDate;
        ArrivalAirportIataCode = arrivalAirportIataCode;
        ArrivalDate = arrivalDate;
        NumberOfBookableSeats = numberOfBookableSeats;
        NumberOfStops = numberOfStops;
        Currency = currency;
        Price = price;
    }

    public string DepartureAirportIataCode { get; }

    public DateTime DepartureDate { get; }

    public string ArrivalAirportIataCode { get; }

    public DateTime ArrivalDate { get; }

    public uint NumberOfBookableSeats { get; }

    public int NumberOfStops { get; }

    public string Currency { get; }

    public double Price { get; }
}
