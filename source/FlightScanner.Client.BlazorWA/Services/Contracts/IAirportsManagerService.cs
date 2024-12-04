using FlightScanner.DTOs.Models;

namespace FlightScanner.Client.BlazorWA.Services.Contracts;

public interface IAirportsManagerService
{
    Task<IReadOnlyList<AirportDto>> GetAllAirports();
}
