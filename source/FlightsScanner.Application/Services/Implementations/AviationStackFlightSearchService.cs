using System.Net.Http.Json;
using FlightScanner.Domain.Exceptions;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace FlightsScanner.Application.Services.Implementations;

public class AviationStackFlightSearchService : IFlightSearchService
{
    private const string AVIATIONSTACK_FLIGHT_SEARCH_CONFIGURATION_KEY = "AviationStackFlightSearchApiKey";
    private const string AVIATIONSTACK_BASE_REQUEST_URI = "https://api.aviationstack.com/v1/flights";
    private const string API_KEY_TERM = "access_key";
    private const string DEPARTURE_AIRPORT_IATA_CODE_HEADER = "dep_iata";
    private const string ARRIVAL_AIRPORT_IATA_CODE_HEADER = "arr_iata";
    private const string DEPARTURE_DATE_HEADER = "flight_date";
    private const string AVIATION_STACK_DATE_TIME_FORMAT = "YYYY-MM-DD";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _aviationstackFlightSearchApiKey;

    public AviationStackFlightSearchService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _aviationstackFlightSearchApiKey = configuration[AVIATIONSTACK_FLIGHT_SEARCH_CONFIGURATION_KEY]
            ?? throw new ArgumentNullException("Aviation stack API key not found!");
    }

    public async Task<FoundFlightsResponseDto> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency)
    {
        var requestUri = $"{AVIATIONSTACK_BASE_REQUEST_URI}?{API_KEY_TERM}={_aviationstackFlightSearchApiKey}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        httpRequestMessage.Headers.Add(API_KEY_TERM, _aviationstackFlightSearchApiKey);
        httpRequestMessage.Headers.Add(DEPARTURE_AIRPORT_IATA_CODE_HEADER, departureAirportIataCode);
        httpRequestMessage.Headers.Add(DEPARTURE_DATE_HEADER, departureTime.ToString(AVIATION_STACK_DATE_TIME_FORMAT));
        httpRequestMessage.Headers.Add(ARRIVAL_AIRPORT_IATA_CODE_HEADER, destinationAirportIataCode);

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
