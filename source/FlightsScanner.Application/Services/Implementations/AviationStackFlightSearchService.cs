using FlightScanner.Domain.DTOs;
using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightScanner.Domain.Services;
using FlightsScanner.Application.Constants;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace FlightsScanner.Application.Services.Implementations;

public class AviationStackFlightSearchService : IFlightSearchService
{
    private const string AVIATIONSTACK_FLIGHT_SEARCH_API_KEY = "AviationStackFlightSearchApiKey";
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
        _aviationstackFlightSearchApiKey = configuration[AVIATIONSTACK_FLIGHT_SEARCH_API_KEY]
            ?? throw new ArgumentNullException("Aviation stack API key not found!");
    }

    public async Task<IReadOnlyList<FlightEntity>> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime returnTripTime, int numberOfPassengers, string currency)
    {
        var requestUri = $"{AVIATIONSTACK_BASE_REQUEST_URI}?{API_KEY_TERM}={_aviationstackFlightSearchApiKey}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        httpRequestMessage.Headers.Add(API_KEY_TERM, _aviationstackFlightSearchApiKey);
        httpRequestMessage.Headers.Add(DEPARTURE_AIRPORT_IATA_CODE_HEADER, departureAirportIataCode);
        httpRequestMessage.Headers.Add(DEPARTURE_DATE_HEADER, departureTime.ToString(AVIATION_STACK_DATE_TIME_FORMAT));
        httpRequestMessage.Headers.Add(ARRIVAL_AIRPORT_IATA_CODE_HEADER, destinationAirportIataCode);

        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.AVIATIONSTACK_CLIENT_NAME);
        var httpResponse = await httpClient.SendAsync(httpRequestMessage);
        httpResponse.EnsureSuccessStatusCode();

        var results = await httpResponse.Content.ReadFromJsonAsync<FoundFlightsCollectionDto>();

        if (results?.FlightInfoDtos is null)
        {
            throw new InvalidResponseException("Flight search didn't find any flights!");
        }

        var flights = new List<FlightEntity>();
        foreach (var flight in results.FlightInfoDtos)
        {
            var flightEntity = new FlightEntity(
                flight.DepartureAirport.Airport,
                flight.DepartureAirport.IataCode,
                flight.DepartureAirport.Sheduled,
                flight.ArrivalAirport.Airport,
                flight.ArrivalAirport.IataCode,
                flight.ArrivalAirport.Sheduled);

            flights.Add(flightEntity);
        }

        return flights;
    }
}
