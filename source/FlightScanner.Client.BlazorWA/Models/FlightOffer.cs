namespace FlightScanner.Client.BlazorWA.Models;

public class FlightOfferViewModel
{
    public FlightOfferViewModel(
        string departureAirportIataCode,
        DateTime departureDate,
        string arrivalAirportIataCode,
        DateTime? returnDate,
        int numberOfBookableSeats,
        int numberOfStops,
        string currency,
        decimal price)
    {
        DepartureAirportIataCode = departureAirportIataCode;
        DepartureDate = departureDate;
        ArrivalAirportIataCode = arrivalAirportIataCode;
        ReturnDate = returnDate;
        NumberOfBookableSeats = numberOfBookableSeats;
        NumberOfStops = numberOfStops;
        Currency = currency;
        Price = price;
    }

    public string DepartureAirportIataCode { get; }

    public DateTime DepartureDate { get; }

    public string ArrivalAirportIataCode { get; }

    public DateTime? ReturnDate { get; }

    public int NumberOfBookableSeats { get; }

    public int NumberOfStops { get; }

    public string Currency { get; }

    public decimal Price { get; }
}
