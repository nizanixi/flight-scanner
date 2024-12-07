﻿using System.Net.Http.Json;
using System.Web;
using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.Common.Constants;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;

namespace FlightScanner.Client.BlazorWA.Services.Implementations;

public class FlightSearchService : IFlightSearchService
{
    private readonly HttpClient _httpClient;

    public FlightSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<FlightEntityDto>> GetAvailableFlights(FlightSearchViewModel flightSearchVM)
    {
        var uriBuilder = new UriBuilder("https://localhost:7021/api/v1/flights");

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query["departureAirportIataCode"] = flightSearchVM.DepartureAirportIataCode;
        query["departureTime"] = flightSearchVM.DepartureDate.ToString(DateTimeConstants.DATE_TIME_FORMAT);
        query["destinationAirportIataCode"] = flightSearchVM.DestionationAirportIataCode;
        if (flightSearchVM.ReturnDate.HasValue)
        {
            query["returnTripTime"] = flightSearchVM.ReturnDate.Value.ToString(DateTimeConstants.DATE_TIME_FORMAT);
        }
        query["numberOfPassengers"] = flightSearchVM.NumberOfPassengers.ToString();
        query["currency"] = flightSearchVM.SelectedCurrency.ToString();

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
