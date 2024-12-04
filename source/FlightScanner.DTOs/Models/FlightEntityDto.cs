namespace FlightScanner.DTOs.Models;

public class FlightEntityDto
{
    public FlightEntityDto(
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
        NumberOfBookableSeats = numberOfBookableSeats;
        NumberOfStops = numberOfStops;
        Currency = currency;
        Price = price;
    }

    public string DepartureAirportIataCode { get; }

    public DateTime DepartureDate { get; }

    public string ArrivalAirportIataCode { get; }

    public DateTime? ReturnDate { get; }

    public int NumberOfStops { get; }

    public int NumberOfBookableSeats { get; }

    public string Currency { get; }

    public decimal Price { get; }
}
