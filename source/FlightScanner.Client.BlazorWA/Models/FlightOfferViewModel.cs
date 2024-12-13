namespace FlightScanner.Client.BlazorWA.Models;

public class FlightOfferViewModel
{
    public FlightOfferViewModel(
        string departureAirportIataCode,
        string departureAirportLocation,
        DateTime departureDate,
        string arrivalAirportIataCode,
        string arrivalAirportLocation,
        DateTime? returnDate,
        int numberOfBookableSeats,
        int numberOfStops,
        string currency,
        decimal price)
    {
        DepartureAirportIataCode = departureAirportIataCode;
        DepartureAirportLocation = departureAirportLocation;
        DepartureDate = departureDate;
        ArrivalAirportIataCode = arrivalAirportIataCode;
        ArrivalAirportLocation = arrivalAirportLocation;
        ReturnDate = returnDate;
        NumberOfBookableSeats = numberOfBookableSeats;
        NumberOfStops = numberOfStops;
        Currency = currency;
        Price = price;
    }

    public string DepartureAirportIataCode { get; }

    public string DepartureAirportLocation { get; }

    public DateTime DepartureDate { get; }

    public string ArrivalAirportIataCode { get; }

    public string ArrivalAirportLocation { get; }

    public DateTime? ReturnDate { get; }

    public int NumberOfBookableSeats { get; }

    public int NumberOfStops { get; }

    public string Currency { get; }

    public decimal Price { get; }
}
