using System.Net.Http.Json;
using FlightScanner.Domain.Exceptions;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;
using FlightsScanner.Application.Configurations;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;

namespace FlightsScanner.Application.Services.Implementations;

public class AviationStackFlightSearchService : IFlightSearchService
{
    private const string AVIATION_STACK_DATE_TIME_FORMAT = "YYYY-MM-DD";

    private readonly AviationEndpointConfiguration _aviationEndpointConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AviationStackFlightSearchService(AviationEndpointConfiguration aviationEndpointConfiguration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _aviationEndpointConfiguration = aviationEndpointConfiguration;
    }

    public async Task<FoundFlightsResponseDto> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency)
    {
        var requestUri = $"{_aviationEndpointConfiguration.BaseUri}?{_aviationEndpointConfiguration.AccessKeyHeader}={_aviationEndpointConfiguration.AviationFlightSearchApiKey}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.AccessKeyHeader, _aviationEndpointConfiguration.AviationFlightSearchApiKey);
        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.DepartureIataCodeHeader, departureAirportIataCode);
        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.FlightDateHeader, departureTime.ToString(AVIATION_STACK_DATE_TIME_FORMAT));
        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.ArrivalIataCodeHeader, destinationAirportIataCode);

        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.DEFAULT_HTTP_CLIENT_NAME);
        var httpResponse = await httpClient.SendAsync(httpRequestMessage);
        httpResponse.EnsureSuccessStatusCode();

        var results = await httpResponse.Content.ReadFromJsonAsync<AviationFlightsResponseDto>();

        if (results?.FlightInfoDtos is null)
        {
            throw new InvalidResponseException("Flight search didn't find any flights!");
        }

        var flights = new List<FlightEntityDto>();
        foreach (var flight in results.FlightInfoDtos)
        {
            var flightEntity = new FlightEntityDto(
                departureAirportIataCode: flight.DepartureAirport.IataCode,
                departureDate: DateTime.Parse(flight.DepartureAirport.Sheduled),
                arrivalAirportIataCode: flight.ArrivalAirport.IataCode,
                returnDate: DateTime.Parse(flight.ArrivalAirport.Sheduled),
                numberOfStops: default,
                numberOfBookableSeats: default,
                currency: string.Empty,
                price: default);

            flights.Add(flightEntity);
        }

        return new FoundFlightsResponseDto(flights);
    }
}
