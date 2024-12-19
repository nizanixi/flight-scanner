using FlightScanner.DTOs.Responses;

namespace FlightsScanner.Application.Interfaces.HttpClients;

public interface IFlightSearchHttpClient
{
    Task<FoundFlightsResponseDto> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency, CancellationToken cancellationToken);
}
