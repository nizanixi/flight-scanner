using System.Net.Http.Json;
using FlightScanner.Client.BlazorWA.Configurations;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;

namespace FlightScanner.Client.BlazorWA.Services.Implementations;

public class AirportsManagerService : IAirportsManagerService
{
    private readonly HttpClient _httpClient;
    private readonly IBlazorWAConfiguration _configuration;

    public AirportsManagerService(HttpClient httpClient, IBlazorWAConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<AirportDto>> GetAllAirports()
    {
        var response = await _httpClient.GetAsync(_configuration.AirportsEndpointConfiguration.GetAllAirportsEndpoint);
        _ = response.EnsureSuccessStatusCode();

        var allAirportsDto = await response.Content.ReadFromJsonAsync<AllAirportsResponseDto>();

        if (allAirportsDto == null)
        {
            return Array.Empty<AirportDto>();
        }

        return allAirportsDto.Airports;
    }
}
