using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.DTOs.Models;

namespace FlightScanner.Client.BlazorWA.Services.Contracts;

public interface IFlightSearchService
{
    Task<IReadOnlyList<FlightEntityDto>> GetAvailableFlights(FlightSearchViewModel flightSearchVM);
}
