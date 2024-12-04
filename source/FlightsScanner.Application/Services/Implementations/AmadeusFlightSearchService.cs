using FlightScanner.Domain.DTOs;
using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightScanner.Domain.Services;
using FlightsScanner.Application.Constants;
using MediatR;
using System.Net.Http.Json;
using System.Web;

namespace FlightsScanner.Application.Services.Implementations;

public class AmadeusFlightSearchService : IFlightSearchService
{
    private const string AMADEUS_BASE_REQUEST_URI = "https://test.api.amadeus.com";
    private const string GET_FLIGHTS_ENDPOINT = "v2/shopping/flight-offers";

    private readonly IHttpClientFactory _httpClientFactory;

    public AmadeusFlightSearchService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IReadOnlyList<FlightEntityDto>> GetFlights(string departureAirportIataCode, DateTime departureTime, string destinationAirportIataCode, DateTime? returnTripTime, int numberOfPassengers, string currency)
    {
        var requestUri = $"{AMADEUS_BASE_REQUEST_URI}/{GET_FLIGHTS_ENDPOINT}";

        var uriBuilder = new UriBuilder(requestUri);

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["originLocationCode"] = departureAirportIataCode;
        query["departureDate"] = departureTime.ToString("yyyy-MM-dd");
        query["destinationLocationCode"] = destinationAirportIataCode;
        if (returnTripTime.HasValue)
        {
            query["returnDate"] = returnTripTime.Value.ToString("yyyy-MM-dd");
        }
        query["adults"] = numberOfPassengers.ToString();
        query["currencyCode"] = currency;
        uriBuilder.Query = query.ToString();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.AMADEUS_CLIENT_NAME);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.amadeus+json");
        var httpResponse = await httpClient.SendAsync(httpRequestMessage);
        httpResponse.EnsureSuccessStatusCode();

        var results = await httpResponse.Content.ReadFromJsonAsync<AmadeusFlightsResponseDto>();

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

            foreach (var flightRoute in flightOffer.FlightRoutes)
            {
                foreach (var segment in flightRoute.Segments)
                {
                    var departureIataCode = segment.DepartureAirport.IataCode;
                    var departureDateTime = segment.DepartureAirport.DateTime;

                    var destinationIataCode = segment.ArrivalAirport.IataCode;
                    var returnDateTime = segment.DepartureAirport.DateTime;

                    var numberOfStops = segment.NumberOfStops;

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
            }
        }

        return flights;
    }
}
