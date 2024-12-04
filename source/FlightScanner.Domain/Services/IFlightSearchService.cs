using FlightScanner.Domain.Entities;

namespace FlightScanner.Domain.Services;

public interface IFlightSearchService
{
    Task<IReadOnlyList<FlightEntityDto>> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency);
}
