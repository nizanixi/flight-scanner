using System.Net.Http.Json;
using System.Web;
using FlightScanner.Client.BlazorWA.Configurations;
using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.Common.Constants;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;

namespace FlightScanner.Client.BlazorWA.Services.Implementations;

public class FlightSearchService : IFlightSearchService
{
    private readonly HttpClient _httpClient;
    private readonly IBlazorWAConfiguration _configuration;

    public FlightSearchService(HttpClient httpClient, IBlazorWAConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<FlightEntityDto>> GetAvailableFlights(FlightSearchViewModel flightSearchVM)
    {
        var uri = $"{_configuration.BackendApiBaseUri}/{_configuration.FlightsEndpointConfiguration.GetFlightEndpoint}";
        var uriBuilder = new UriBuilder(uri);

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query[_configuration.FlightsEndpointConfiguration.DepartureAirportQueryString] = flightSearchVM.DepartureAirportIataCode;
        query[_configuration.FlightsEndpointConfiguration.DepartureTimeQueryString] = flightSearchVM.DepartureDate.ToString(DateTimeConstants.DATE_TIME_FORMAT);
        query[_configuration.FlightsEndpointConfiguration.DestinationAirportQueryString] = flightSearchVM.DestionationAirportIataCode;
        if (flightSearchVM.ReturnDate.HasValue)
        {
            query[_configuration.FlightsEndpointConfiguration.ReturnTimeQueryString] = flightSearchVM.ReturnDate.Value.ToString(DateTimeConstants.DATE_TIME_FORMAT);
        }
        query[_configuration.FlightsEndpointConfiguration.NumberOfPassengersQueryString] = flightSearchVM.NumberOfPassengers.ToString();
        query[_configuration.FlightsEndpointConfiguration.CurrencyQueryString] = flightSearchVM.SelectedCurrency.ToString();

        uriBuilder.Query = query.ToString();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

        var response = await _httpClient.SendAsync(httpRequestMessage);
        _ = response.EnsureSuccessStatusCode();

        var foundFlightsDto = await response.Content.ReadFromJsonAsync<FoundFlightsResponseDto>();

        if (foundFlightsDto == null)
        {
            return Array.Empty<FlightEntityDto>();
        }

        return foundFlightsDto.FlightEntityDtos;
    }
}
