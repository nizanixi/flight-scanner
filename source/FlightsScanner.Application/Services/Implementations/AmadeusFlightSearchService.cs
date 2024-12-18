using System.Net.Http.Json;
using System.Web;
using FlightScanner.Common.Constants;
using FlightScanner.Domain.Exceptions;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;
using FlightsScanner.Application.Configurations;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;

namespace FlightsScanner.Application.Services.Implementations;

public class AmadeusFlightSearchService : IFlightSearchService
{
    private const string AMADEUS_API_DATE_TIME_FORMAT = "yyyy-MM-dd";
    private const int DEPARTURE_AIRPORT = 1;
    private const int DESTINATION_AIRPORT = 1;
    private const int ALREADY_INCLUDED_ROUTES_COUNT = DEPARTURE_AIRPORT + DESTINATION_AIRPORT;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AmadeusEndpointConfiguration _amadeusEndpointConfiguration;

    public AmadeusFlightSearchService(IHttpClientFactory httpClientFactory, AmadeusEndpointConfiguration amadeusEndpointConfiguration)
    {
        _httpClientFactory = httpClientFactory;
        _amadeusEndpointConfiguration = amadeusEndpointConfiguration;
    }

    public async Task<FoundFlightsResponseDto> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency, CancellationToken cancellationToken)
    {
        var requestUri = $"{_amadeusEndpointConfiguration.BaseUri}/{_amadeusEndpointConfiguration.GetFlightEndpoint}";

        var uriBuilder = new UriBuilder(requestUri);

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query[_amadeusEndpointConfiguration.OriginLocationCodeQueryString] = departureAirportIataCode;
        query[_amadeusEndpointConfiguration.DepartureDateQueryString] = departureTime.ToString(AMADEUS_API_DATE_TIME_FORMAT);
        query[_amadeusEndpointConfiguration.DestinationLocationCodeQueryString] = destinationAirportIataCode;
        if (returnTripTime.HasValue)
        {
            query[_amadeusEndpointConfiguration.ReturnDateQueryString] = returnTripTime.Value.ToString(AMADEUS_API_DATE_TIME_FORMAT);
        }
        query[_amadeusEndpointConfiguration.AdultsQueryString] = numberOfPassengers.ToString();
        query[_amadeusEndpointConfiguration.CurrencyCodeQueryString] = currency;
        uriBuilder.Query = query.ToString();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.AMADEUS_CLIENT_NAME);
        httpClient.DefaultRequestHeaders.Add(HttpHeaderConstants.ACCEPT_TYPE, HttpHeaderConstants.AMADEUS_JSON_TYPE);
        var httpResponse = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var results = await httpResponse.Content.ReadFromJsonAsync<AmadeusFlightsResponseDto>(cancellationToken);

        if (results?.FlightOffers is null)
        {
            throw new InvalidResponseException("Flight search didn't find any flights!");
        }

        var flights = new List<FlightEntityDto>();
        foreach (var flightOffer in results.FlightOffers)
        {
            var price = flightOffer.Price.Total;
            var flightCurrency = flightOffer.Price.Currency;
            var numberOfBookableSeats = flightOffer.NumberOfBookableSeats;

            // Second value in flightOffer.FlightRoutes is going back route
            var flightRouteInSearchedDirection = flightOffer.FlightRoutes.First();

            var numberOfStops = flightRouteInSearchedDirection.RouteSegments.Count - ALREADY_INCLUDED_ROUTES_COUNT;

            var departureIataCode = flightRouteInSearchedDirection.RouteSegments.First().DepartureAirport.IataCode;
            var departureDateTime = flightRouteInSearchedDirection.RouteSegments.First().DepartureAirport.DateTime;

            var destinationIataCode = flightRouteInSearchedDirection.RouteSegments.Last().ArrivalAirport.IataCode;
            var returnDateTime = flightRouteInSearchedDirection.RouteSegments.Last().ArrivalAirport.DateTime;

            foreach (var flightRoute in flightOffer.FlightRoutes)
            {
                foreach (var segment in flightRoute.RouteSegments)
                {
                    numberOfStops += segment.NumberOfStops;
                }
            }

            var flightEntity = new FlightEntityDto(
                departureAirportIataCode: departureIataCode,
                departureDate: departureDateTime,
                arrivalAirportIataCode: destinationAirportIataCode,
                returnDate: returnDateTime,
                numberOfStops: numberOfStops,
                numberOfBookableSeats: numberOfBookableSeats,
                currency: flightCurrency,
                price: price);

            flights.Add(flightEntity);
        }

        return new FoundFlightsResponseDto(flights);
    }
}
