using FlightScanner.Domain.Entities;

namespace FlightScanner.Domain.Services;

public interface IFlightSearchService
{
    Task<IReadOnlyList<FlightEntity>> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime returnTripTime, int numberOfPassengers, string currency);
}
