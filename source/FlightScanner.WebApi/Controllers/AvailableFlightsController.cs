using FlightScanner.DTOs.Exceptions;
using FlightScanner.DTOs.Responses;
using FlightScanner.WebApi.Validation;
using FlightsScanner.Application.Flights.Queries.GetFlights;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FlightScanner.WebApi.Controllers;

[ApiController]
[Route("api/v1")]
public class AvailableFlightsController : ControllerBase
{
    private readonly ISender _sender;

    public AvailableFlightsController(ISender sender)
    {
        _sender = sender;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FoundFlightsResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDto))]
    [HttpGet]
    [Route("flights")]
    public async Task<IActionResult> GetAvailableFlights(
        [FromQuery][IataCodeValidation] string departureAirportIataCode,
        [FromQuery][DepartureDateTimeValidation] string departureTime,
        [FromQuery][IataCodeValidation] string destinationAirportIataCode,
        [FromQuery][ArrivalDateTimeValidation] string returnTripTime,
        [FromQuery][PositiveNumberValidation] int numberOfPassengers,
        [FromQuery][CurrencyValidation] string currency,
        CancellationToken cancellationToken)
    {
        var getFlightsQuery = new GetFlightsQuery(
            departureAirportIataCode: departureAirportIataCode,
            departureTime: DateTime.Parse(departureTime),
            destinationAirportIataCode: destinationAirportIataCode,
            returnTripTime: DateTime.Parse(returnTripTime),
            numberOfPassengers: numberOfPassengers,
            currency: currency);

        var flightOffers = await _sender.Send(
            request: getFlightsQuery,
            cancellationToken: cancellationToken);

        return Ok(flightOffers);
    }
}
