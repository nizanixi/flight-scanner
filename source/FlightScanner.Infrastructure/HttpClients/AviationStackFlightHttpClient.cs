using System.Net.Http.Json;
using FlightScanner.Domain.Exceptions;
using FlightScanner.Domain.Models;
using FlightScanner.DTOs.Responses;
using FlightScanner.Infrastructure.Configurations;
using FlightScanner.Infrastructure.Constants;
using FlightsScanner.Application.Interfaces.HttpClients;

namespace FlightScanner.Infrastructure.HttpClients;

public class AviationStackFlightHttpClient : IFlightSearchHttpClient
{
    private const string AVIATION_STACK_DATE_TIME_FORMAT = "YYYY-MM-DD";

    private readonly AviationEndpointConfiguration _aviationEndpointConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AviationStackFlightHttpClient(AviationEndpointConfiguration aviationEndpointConfiguration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _aviationEndpointConfiguration = aviationEndpointConfiguration;
    }

    public async Task<IReadOnlyList<FlightInformation>> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency, CancellationToken cancellationToken)
    {
        var requestUri = $"{_aviationEndpointConfiguration.BaseUri}?{_aviationEndpointConfiguration.AccessKeyHeader}={_aviationEndpointConfiguration.AviationFlightSearchApiKey}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.AccessKeyHeader, _aviationEndpointConfiguration.AviationFlightSearchApiKey);
        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.DepartureIataCodeHeader, departureAirportIataCode);
        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.FlightDateHeader, departureTime.ToString(AVIATION_STACK_DATE_TIME_FORMAT));
        httpRequestMessage.Headers.Add(_aviationEndpointConfiguration.ArrivalIataCodeHeader, destinationAirportIataCode);

        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.DEFAULT_HTTP_CLIENT_NAME);
        var httpResponse = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var results = await httpResponse.Content.ReadFromJsonAsync<AviationFlightsResponseDto>(cancellationToken);

        if (results?.FlightInfoDtos is null)
        {
            throw new InvalidResponseException("Flight search didn't find any flights!");
        }

        var flights = new List<FlightInformation>();
        foreach (var flight in results.FlightInfoDtos)
        {
            var flightInformation = new FlightInformation(
                departureAirportIataCode: flight.DepartureAirport.IataCode,
                departureDate: DateTime.Parse(flight.DepartureAirport.Sheduled),
                arrivalAirportIataCode: flight.ArrivalAirport.IataCode,
                returnDate: DateTime.Parse(flight.ArrivalAirport.Sheduled),
                numberOfStops: default,
                numberOfBookableSeats: default,
                currency: string.Empty,
                price: default);

            flights.Add(flightInformation);
        }

        return flights;
    }
}
