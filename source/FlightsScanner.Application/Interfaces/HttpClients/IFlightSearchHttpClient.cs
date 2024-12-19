using FlightScanner.Domain.Models;

namespace FlightsScanner.Application.Interfaces.HttpClients;

public interface IFlightSearchHttpClient
{
    Task<IReadOnlyList<FlightInformation>> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency, CancellationToken cancellationToken);
}
