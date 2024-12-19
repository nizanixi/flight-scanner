namespace FlightScanner.Domain.Models;

public class FlightInformation
{
    public FlightInformation(
        string departureAirportIataCode,
        DateTime departureDate,
        string arrivalAirportIataCode,
        DateTime? returnDate,
        int numberOfStops,
        int numberOfBookableSeats,
        string currency,
        decimal price)
    {
        DepartureAirportIataCode = departureAirportIataCode;
        DepartureDate = departureDate;
        ArrivalAirportIataCode = arrivalAirportIataCode;
        ReturnDate = returnDate;
        NumberOfStops = numberOfStops;
        NumberOfBookableSeats = numberOfBookableSeats;
        Currency = currency;
        Price = price;
    }

    public string DepartureAirportIataCode { get; }

    public string DepartureAirportLocation { get; set; } = null!;

    public DateTime DepartureDate { get; }

    public string ArrivalAirportIataCode { get; }

    public string ArrivalAirportLocation { get; set; } = null!;

    public DateTime? ReturnDate { get; }

    public int NumberOfStops { get; }

    public int NumberOfBookableSeats { get; }

    public string Currency { get; }

    public decimal Price { get; }
}
