using FlightScanner.Domain.Entities;
using MediatR;

namespace FlightsScanner.Application.Flights.Queries.GetFlights;

public class GetFlightsQuery : IRequest<IReadOnlyList<FlightEntity>>
{
    public GetFlightsQuery(
        string departureAirportIataCode,
        DateTime departureTime,
        string destinationAirportIataCode,
        DateTime returnTripTime,
        int numberOfPassengers,
        string currency)
    {
        DepartureAirportIataCode = departureAirportIataCode;
        DepartureTime = departureTime;
        DestinationAirportIataCode = destinationAirportIataCode;
        ReturnTripTime = returnTripTime;
        NumberOfPassengers = numberOfPassengers;
        Currency = currency;
    }

    public string DepartureAirportIataCode { get; }

    public DateTime DepartureTime { get; }

    public string DestinationAirportIataCode { get; }

    public DateTime ReturnTripTime { get; }

    public int NumberOfPassengers { get; }

    public string Currency { get; }
}
