using System.Net.Http.Json;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;

namespace FlightScanner.Client.BlazorWA.Services.Implementations;

public class AirportsManagerService : IAirportsManagerService
{
    private readonly HttpClient _httpClient;

    public AirportsManagerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<AirportDto>> GetAllAirports()
    {
        var response = await _httpClient.GetAsync("api/v2/all-airports");
        _ = response.EnsureSuccessStatusCode();

        var allAirportsDto = await response.Content.ReadFromJsonAsync<AllAirportsResponseDto>();

        if (allAirportsDto == null)
        {
            return Array.Empty<AirportDto>();
        }

        return allAirportsDto.Airports;
    }
}
