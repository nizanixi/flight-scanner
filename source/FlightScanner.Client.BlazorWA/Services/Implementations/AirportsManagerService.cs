using System.Net.Http.Json;
using FlightScanner.Client.BlazorWA.Configurations;
using FlightScanner.Client.BlazorWA.Constants;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;

namespace FlightScanner.Client.BlazorWA.Services.Implementations;

public class AirportsManagerService : IAirportsManagerService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IBlazorWAConfiguration _configuration;

    public AirportsManagerService(IHttpClientFactory httpClientFactory, IBlazorWAConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<AirportDto>> GetAllAirports()
    {
        var httpClient = _httpClientFactory.CreateClient(HttpConstants.BACKEND_HTTP_CLIENT_NAME);

        var response = await httpClient.GetAsync(_configuration.AirportsEndpointConfiguration.GetAllAirportsEndpoint);
        _ = response.EnsureSuccessStatusCode();

        var allAirportsDto = await response.Content.ReadFromJsonAsync<AllAirportsResponseDto>();

        if (allAirportsDto == null)
        {
            return Array.Empty<AirportDto>();
        }

        return allAirportsDto.Airports;
    }
}
