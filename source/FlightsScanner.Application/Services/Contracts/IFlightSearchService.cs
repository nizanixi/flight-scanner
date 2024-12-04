using FlightScanner.DTOs.Responses;

namespace FlightsScanner.Application.Services.Contracts;

public interface IFlightSearchService
{
    Task<FoundFlightsResponseDto> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency);
}
